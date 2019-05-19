import { JsonParse, JsonParseResult, JsonQuery } from "./json-query";
import { JsonQueryResult } from "./json-query-result";
import { JsonParseError } from "./json-parse-error";
import { node } from "./json-node";
import { QueryVisitor } from "./json-query-visitor";

export const map = (parseProps: JsonParse[]) => (obj: any, path: string = ""): JsonParseResult => {
    if (obj) {
        const results = obj ? parseProps.map(parseProp => parseProp(obj, path)) : [];

        return node(results, (queries: JsonQuery[]) => (visitor: QueryVisitor) =>
            visitor.found ?
                [[new JsonQueryResult(obj, visitor.path)]] :
                queries.map(query => query.findInternal(visitor)));
    }
    return [new JsonParseError("object is expected", path)];
};