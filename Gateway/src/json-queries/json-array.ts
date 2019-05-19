import { JsonParse, JsonParseResult, JsonQuery } from "./json-query";
import { JsonQueryResult } from "./json-query-result";
import { JsonParseError } from "./json-parse-error";
import { node } from "./json-node";
import { QueryVisitor } from "./json-query-visitor";

export const array = (parseItem: JsonParse) => (obj: any, path: string = ""): JsonParseResult => {
    if (obj && obj instanceof Array) {
        const results = obj.map((item: any, index: number) => parseItem(item, `${path}/${index}`));

        return node(results, (queries: JsonQuery[]) => (visitor: QueryVisitor) =>
            visitor.found ?
                [[new JsonQueryResult(obj, visitor.path)]] :
                queries.map((query, index: number) => {
                    if (visitor.goDown(index.toString())) {
                        const result = query.findInternal(visitor);
                        visitor.goUp();
                        return result;
                    }
                    return [];
                }));
    }
    return [new JsonParseError("array is expected", path)];
};
