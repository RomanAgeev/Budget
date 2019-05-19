import { JsonParse, JsonParseResult, JsonQuery } from "./json-query";
import { JsonParseError } from "./json-parse-error";
import { node } from "./json-node";
import { QueryVisitor } from "./json-query-visitor";

export const prop = (name: string, parseValue: JsonParse) => (obj: any, path: string = ""): JsonParseResult => {
    if (name !== "*" && !obj[name]) {
        return [new JsonParseError(`property ${name} is expected`, path)]
    }

    const propNames = name === "*" ? Object.getOwnPropertyNames(obj) : [name];

    const results = propNames.map(propName => parseValue(obj[propName], `${path}/${propName}`));

    return node(results, (queries: JsonQuery[]) => (visitor: QueryVisitor) =>
        queries.map((query, index: number) => {
            if (visitor.goDown(propNames[index])) {
                const result = query.findInternal(visitor);
                visitor.goUp();
                return result;
            }
            return [];
        }));
};
