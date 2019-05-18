import fs from "fs";
import util from "util";
import yaml from "js-yaml";
import UrlPattern from "url-pattern";
import { JsonParseError, flattenErrors, isJsonParseErrors, JsonQuery, JsonQueryResult } from "../json-queries";

let settings: any = null;

export const routeMatcher = (settingsPath: string) => async (publicRoute: string): Promise<string | null> => {
    await ensureSettings(settingsPath);
    // TODO: implement route matcher
    return null;
};

const ensureSettings = async (settingsPath: string): Promise<void> => {
    if (!settings) {
        const settingsYaml: string = await readFileAsync(settingsPath, "utf8");
        settings = yaml.safeLoad(settingsYaml);

        const jsonQuery = parseSettings(settings);
        if (jsonQuery instanceof JsonQuery) {
            const results = jsonQuery.findMany("services/*/routes/*/public");
            if (results.length > 0) {
                console.log("public:" + JSON.stringify(results[0]));

                const _private = jsonQuery.findMany(results[0].path + "/./private");
                console.log("private:" + JSON.stringify(_private));

                const host = jsonQuery.findMany(results[0].path + "/./././host");
                console.log("host:" + JSON.stringify(host));
            }
        }
    }
};

const readFileAsync = util.promisify(fs.readFile);

type Check = (obj: any, position: string[]) => JsonQuery | JsonParseError[];

// type QueryPredicate = (value: any) => boolean;

type valueType = "string" | "number" | "boolean";

const value = (type: valueType) => (obj: any, position: string[] = []): JsonQuery | JsonParseError[] => {
    if (typeof obj === "string") {
        return new JsonQuery(request => request.found ?
            [new JsonQueryResult(obj, request.currentPath)] :
            []);
    }
    return [new JsonParseError("string value is expected", position)];
};

const list = (check: Check) => (obj: any, position: string[] = []): JsonQuery | JsonParseError[] => {
    if (obj instanceof Array) {
        const queries = obj.map((it, index: number) => check(it, position.concat(index.toString())));

        const errorsList = queries.filter(it => isJsonParseErrors(it)) as JsonParseError[][];
        if (errorsList.length > 0) {
            return flattenErrors(errorsList);
        }

        return new JsonQuery(request => {
            if (request.found) {
                return [new JsonQueryResult(obj, request.currentPath)];
            }
            return (queries as JsonQuery[]).map((it, index: number) => {
                if (request.goDown(index.toString())) {
                    const result = it.findInternal(request);
                    request.goUp();
                    return result;
                }
                return [];
            })
            .reduce((results, acc) => acc.concat(results), []);
        });
    }
    return [new JsonParseError("array is expected", position)];
};

const map = (checks: Check[]) => (obj: any, position: string[] = []): JsonQuery | JsonParseError[] => {
    const queries = obj ? checks.map(check => check(obj, position)) : [];

    const errorsList = queries.filter(it => isJsonParseErrors(it)) as JsonParseError[][];
    if (errorsList.length > 0) {
        return flattenErrors(errorsList);
    }

    return new JsonQuery(request => {
        if (request.found) {
            return [new JsonQueryResult(obj, request.currentPath)];
        }
        return (queries as JsonQuery[])
            .map(it => it.findInternal(request))
            .reduce((results, acc) => acc.concat(results), []);
    });
};

const prop = (name: string, checkValue: Check) => (obj: any, position: string[] = []): JsonQuery | JsonParseError[] => {
    const names = name === "*" ? Object.getOwnPropertyNames(obj) : [name];

    const queries = names.map(it => obj[it] ?
        checkValue(obj[it], position.concat(it)) :
        [new JsonParseError(`property ${it} is expected`, position)]);

    const errorsList = queries.filter(it => isJsonParseErrors(it)) as JsonParseError[][];
    if (errorsList.length > 0) {
        return flattenErrors(errorsList);
    }

    return new JsonQuery(request => {
        return (queries as JsonQuery[]).map((it, index: number) => {
            if (request.goDown(names[index])) {
                const result = it.findInternal(request);
                request.goUp();
                return result;
            }
            return [];
        })
        .reduce((results, acc) => acc.concat(results), []);
    });
};

const parseSettings = map([
    prop("services", map([
        prop("*", map([
            prop("host", value("string")),
            prop("routes", list(map([
                prop("public", value("string")),
                prop("private", value("string")),
            ]))),
        ])),
    ])),
]);
