import { JsonParse, JsonParseResult, JsonQuery, QueryRequest } from "./json-query";
import { JsonQueryResult } from "./json-query-result";
import { JsonParseError } from "./json-parse-error";
import { node } from "./json-node";

export const map = (parseProps: JsonParse[]) => (obj: any, path: string = ""): JsonParseResult => {
    if (obj) {
        const results = obj ? parseProps.map(parseProp => parseProp(obj, path)) : [];

        return node(results, (queries: JsonQuery[]) =>
            (request: QueryRequest) => request.found ?
                [[new JsonQueryResult(obj, request.currentPath)]] :
                queries.map(query => query.findInternal(request)));
    }
    return [new JsonParseError("object is expected", path)];
};