import { JsonQueryHandler, JsonParseResult, JsonQuery } from "./json-query";
import { flattenErrors, isJsonParseErrors, JsonParseError } from "./json-parse-error";

type JsonParseGroups = { ok: JsonQuery[], err: JsonParseError[][] };

export const node = (childrenResults: JsonParseResult[], queryHandler: (queries: JsonQuery[]) => JsonQueryHandler): JsonParseResult => {
    const groups: JsonParseGroups = childrenResults.reduce((acc: JsonParseGroups, result: JsonParseResult) =>
        isJsonParseErrors(result) ?
            { ...acc, err: acc.err.concat(result) } :
            { ...acc, ok: acc.ok.concat(result) },
        { ok: [], err: [] });

    return groups.err.length > 0 ? flattenErrors(groups.err) : new JsonQuery(queryHandler(groups.ok));
};
