import { JsonParse, JsonParseResult, JsonQuery, QueryRequest } from "./json-query";
import { JsonQueryResult } from "./json-query-result";
import { JsonParseError } from "./json-parse-error";
import { node } from "./json-node";

export const array = (parseItem: JsonParse) => (obj: any, path: string = ""): JsonParseResult => {
    if (obj && obj instanceof Array) {
        const results = obj.map((item: any, index: number) => parseItem(item, `${path}/${index}`));

        return node(results, (queries: JsonQuery[]) =>
            (request: QueryRequest) => request.found ?
                [[new JsonQueryResult(obj, request.currentPath)]] :
                queries.map((query, index: number) => {
                    if (request.goDown(index.toString())) {
                        const result = query.findInternal(request);
                        request.goUp();
                        return result;
                    }
                    return [];
                }));
    }
    return [new JsonParseError("array is expected", path)];
};
