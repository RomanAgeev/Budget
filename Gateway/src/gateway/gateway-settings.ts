import fs from "fs";
import util from "util";
import yaml from "js-yaml";
import UrlPattern from "url-pattern";

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

        const settingsReducer = parseSettings(settings, []);
        if (typeof settingsReducer === "function") {
            const results = settingsReducer(new QueryRequest("services/*/routes/*/public"));
            if (results.length > 0) {
                console.log("public:" + JSON.stringify(results[0]));
                const _private = settingsReducer(new QueryRequest(results[0].path + "/./private"));
                console.log("private:" + JSON.stringify(_private));
                const host = settingsReducer(new QueryRequest(results[0].path + "/./././host"));
                console.log("host:" + JSON.stringify(host));
            }
        }
    }
};

const readFileAsync = util.promisify(fs.readFile);

type Check = (obj: any, position: string[]) => Query | ParseError[];
type ParseError = { path: string, message: string };
type Query = (request: QueryRequest) => IQueryResult[];
// type QueryPredicate = (value: any) => boolean;

class QueryRequest {
    constructor(pathStr: string) {
        const arr = pathStr.split("/");
        for (const seg of arr) {
            if (seg === ".") {
                this._path.pop();
            } else {
                this._path.push(seg);
            }
        }
    }

    private readonly _path: string[] = [];
    private _currentPath: string[] = [];

    public get currentPath() {
        return this._currentPath.join("/");
    }

    public get found(): boolean {
        return this._index === this._path.length;
    }

    private get _index(): number {
        return this._currentPath.length;
    }

    public goDown(name: string): boolean {
        if (this._matchName(name)) {
            this._currentPath.push(name);
            return true;
        }
        return false;
    }

    public goUp(): void {
        this._currentPath.pop();
    }

    private _matchName(name: string): boolean {
        const pathName: string = this._path[this._index];
        return pathName === "*" || pathName === name;
    }
}

interface IQueryResult {
    readonly path: string;
    readonly value: any;
}

const text = (value: any, position: string[]): Query | ParseError[] => {
    if (typeof value === "string") {
        return (request: QueryRequest): IQueryResult[] => {
            if (request.found) {
                return [{
                    path: request.currentPath,
                    value,
                }];
            }
            return [];
        };
    }
    return [{
        path: position.join("/"),
        message: "string value is expected",
    }];
};

const list = (check: Check) => (value: any, position: string[]): Query | ParseError[] => {
    if (value instanceof Array) {
        const queries = value.map((it, index: number) => check(it, position.concat(index.toString())));

        const errors = queries.filter(it => Array.isArray(it)) as ParseError[][];
        if (errors.length > 0) {
            return errors.reduce((error: ParseError[], acc: ParseError[]) => acc.concat(error), []);
        }
        // if (queries.some(it => it === null)) {
        //     return null;
        // }

        return (request: QueryRequest): IQueryResult[] => {
            if (request.found) {
                return [{
                    path: request.currentPath,
                    value,
                }];
            }
            return (queries as Query[]).map((it, index: number) => {
                if (request.goDown(index.toString())) {
                    const result = it!(request);
                    request.goUp();
                    return result;
                }
                return [];
            })
            .reduce((results: IQueryResult[], acc: IQueryResult[]) => acc.concat(results), []);
        };
    }
    return [{
        path: position.join("/"),
        message: "array is expected",
    }];
};

const map = (checks: Check[]) => (value: any, position: string[]): Query | ParseError[] => {
    const queries = value ? checks.map(check => check(value, position)) : [];

    const errors = queries.filter(it => Array.isArray(it)) as ParseError[][];
    if (errors.length > 0) {
        return errors.reduce((error: ParseError[], acc: ParseError[]) => acc.concat(error), []);
    }

    // if (queries.some(it => it === null)) {
    //     return null;
    // }

    return (request: QueryRequest): IQueryResult[] => {
        if (request.found) {
            return [{
                path: request.currentPath,
                value,
            }];
        }
        return (queries as Query[])
            .map(it => it!(request))
            .reduce((results: IQueryResult[], acc: IQueryResult[]) => acc.concat(results), []);
    };
};

const prop = (name: string, checkValue: Check) => (obj: any, position: string[]): Query | ParseError[] => {
    const names = name === "*" ? Object.getOwnPropertyNames(obj) : [name];

    const queries = names.map(it => obj[it] ? checkValue(obj[it], position.concat(it)) : [{
        path: position.join("/"),
        message: `property ${it} is expected`,
    }]);

    const errors = queries.filter(it => Array.isArray(it)) as ParseError[][];
    if (errors.length > 0) {
        return errors.reduce((error: ParseError[], acc: ParseError[]) => acc.concat(error), []);
    }

    // if (queries.some(it => it === null)) {
    //     return null;
    // }

    return (request: QueryRequest): IQueryResult[] => {
        return (queries as Query[]).map((it, index: number) => {
            if (request.goDown(names[index])) {
                const result = it!(request);
                request.goUp();
                return result;
            }
            return [];
        })
        .reduce((results: IQueryResult[], acc: IQueryResult[]) => acc.concat(results), []);
    };
};

const parseSettings = map([
    prop("services", map([
        prop("*", map([
            prop("host", text),
            prop("routes", list(map([
                prop("public", text),
                prop("private", text),
            ]))),
        ])),
    ])),
]);
