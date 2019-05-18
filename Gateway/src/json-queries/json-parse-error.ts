export class JsonParseError {
    constructor(
        public readonly message: string,
        public readonly path: string) {
    }
}

export const flattenErrors = (errorsList: JsonParseError[][]): JsonParseError[] =>
    errorsList.length > 0 ?
        errorsList.reduce((acc: JsonParseError[], errors: JsonParseError[]) =>
            acc.concat(errors), []) : [];

export const isJsonParseErrors = (value: any): value is JsonParseError[] =>
    value instanceof Array && (value.length === 0 || value[0] instanceof JsonParseError);
