import { JsonParse, JsonParseResult, JsonQuery, QueryRequest } from "./json-query";
import { JsonParseError } from "./json-parse-error";
import { node } from "./json-node";

export const prop = (name: string, parseValue: JsonParse) => (obj: any, path: string = ""): JsonParseResult => {
    if (name !== "*" && !obj[name]) {
        return [new JsonParseError(`property ${name} is expected`, path)]
    }

    const propNames = name === "*" ? Object.getOwnPropertyNames(obj) : [name];

    const results = propNames.map(propName => parseValue(obj[propName], `${path}/${propName}`));

    return node(results, (queries: JsonQuery[]) =>
        (request: QueryRequest) =>
            queries.map((query, index: number) => {
                if (request.goDown(propNames[index])) {
                    const result = query.findInternal(request);
                    request.goUp();
                    return result;
                }
                return [];
            }));
};
