import { JsonQuery, JsonParseResult } from "./json-query";
import { JsonParseError } from "./json-parse-error";
import { JsonQueryResult } from "./json-query-result";

export type jsonValueType = "string" | "number" | "boolean";

export const value = (type: jsonValueType) => (obj: any, path: string = ""): JsonParseResult =>
    typeof obj === type ?
        new JsonQuery(request => [request.found ? [new JsonQueryResult(obj, request.currentPath)] : []]) :
        [new JsonParseError("string value is expected", path)];
