import { JsonQueryResult } from "./json-query-result";
import { JsonParseError } from "./json-parse-error";

export class JsonQuery {
    constructor(
        private readonly _handler: JsonQueryHandler) {
    }

    findInternal(request: QueryRequest): JsonQueryResult[] {
        return this._handler(request)
            .reduce((results, acc) => acc.concat(results), []);
    }

    findMany(path: string): JsonQueryResult[] {
        return this.findInternal(new QueryRequest(path));
    }
}

export type JsonQueryHandler = (request: QueryRequest) => JsonQueryResult[][];

export type JsonParse = (obj: any, path: string) => JsonParseResult;

export type JsonParseResult = JsonQuery | JsonParseError[];


export class QueryRequest {
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