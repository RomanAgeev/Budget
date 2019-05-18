import fs from "fs";
import util from "util";
import yaml from "js-yaml";
import UrlPattern from "url-pattern";
import { JsonParseError, flattenErrors, isJsonParseErrors, JsonQuery, JsonQueryResult, value, QueryRequest, node, JsonParseResult, JsonParse } from "../json-queries";

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


// type QueryPredicate = (value: any) => boolean;


const array = (parseItem: JsonParse) => (obj: any, path: string = ""): JsonParseResult => {
    if (obj && obj instanceof Array) {
        const results = obj.map((item: any, index: number) => parseItem(item, `${path}/${index}`));

        return node(results, request => {
            if (request.found) {
                return [[new JsonQueryResult(obj, request.currentPath)]];
            }
            return (results as JsonQuery[]).map((it, index: number) => {
                if (request.goDown(index.toString())) {
                    const result = it.findInternal(request);
                    request.goUp();
                    return result;
                }
                return [];
            });
        });
    }
    return [new JsonParseError("array is expected", path)];
};

const map = (parseProps: JsonParse[]) => (obj: any, path: string = ""): JsonParseResult => {
    if (obj) {
        const results = obj ? parseProps.map(parseProp => parseProp(obj, path)) : [];

        return node(
            results,
            request => {
                if (request.found) {
                    return [[new JsonQueryResult(obj, request.currentPath)]];
                }
                return (results as JsonQuery[])
                    .map(it => it.findInternal(request));
            });
    }
    return [new JsonParseError("object is expected", path)];
};

const prop = (name: string, parseValue: JsonParse) => (obj: any, path: string = ""): JsonParseResult => {
    if (name !== "*" && !obj[name]) {
        return [new JsonParseError(`property ${name} is expected`, path)]
    }

    const propNames = name === "*" ? Object.getOwnPropertyNames(obj) : [name];

    const results = propNames.map(propName => parseValue(obj[propName], `${path}/${propName}`));

    return node(
        results,
        request => {
            return (results as JsonQuery[]).map((it, index: number) => {
                if (request.goDown(propNames[index])) {
                    const result = it.findInternal(request);
                    request.goUp();
                    return result;
                }
                return [];
            });
        });
};

const parseSettings = map([
    prop("services", map([
        prop("*", map([
            prop("host", value("string")),
            prop("routes", array(map([
                prop("public", value("string")),
                prop("private", value("string")),
            ]))),
        ])),
    ])),
]);
