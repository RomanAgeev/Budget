import { JsonQueryResult } from "./json-query-result";
import { JsonParseError } from "./json-parse-error";
import { QueryVisitor } from "./json-query-visitor";

export class JsonQuery {
    constructor(
        private readonly _handler: JsonQueryHandler) {
    }

    findInternal(visitor: QueryVisitor): JsonQueryResult[] {
        return this._handler(visitor)
            .reduce((acc, results) => acc.concat(results), []);
    }

    findMany(path: string): JsonQueryResult[] {
        const segments = this._normalizeSegments(path.split("/"));
        return this.findInternal(new QueryVisitor(segments));
    }

    private _normalizeSegments(segments: string[]): string[] {
        const result: string[] = [];
        for (const segment of segments) {
            if (segment === ".") {
                result.pop();
            } else {
                result.push(segment);
            }
        }
        return result;
    }
}

export type JsonQueryHandler = (visitor: QueryVisitor) => JsonQueryResult[][];

export type JsonParse = (obj: any, path: string) => JsonParseResult;

export type JsonParseResult = JsonQuery | JsonParseError[];
