import { JsonQueryHandler, JsonParseResult, JsonQuery } from "./json-query";
import { flattenErrors, isJsonParseErrors, JsonParseError } from "./json-parse-error";

export const node = (childrenResults: JsonParseResult[], queryHandler: JsonQueryHandler): JsonParseResult => {
    const errorsList = childrenResults
        .filter(result => isJsonParseErrors(result))
        .map(result => result as JsonParseError[]);

    return errorsList.length > 0 ? flattenErrors(errorsList) : new JsonQuery(queryHandler);
};
