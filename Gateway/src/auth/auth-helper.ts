import { Query as Settings, QueryResult } from "@ra/json-queries";

export const authRequired = (settings: Settings): boolean => settings("authentication").length > 0;

export function getAuthSecret(settings: Settings): string {
    const secrets: QueryResult[] = settings("authentication/secret");
    if (secrets.length === 0) {
        throw new Error("No Authentication secret is specified for the gateway");
    }
    return secrets[0].value;
}
