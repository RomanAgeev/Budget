import fs from "fs";
import util from "util";
import yaml from "js-yaml";
import { json, prop, obj, propAny, value, array, propOptional, ParseError, Query, QueryResult } from "@ra/json-queries";
import { Predicate } from "@ra/json-queries/dist/iterate";

export interface Settings {
    getRouteParams(apiUrl: string, predicate: (api: string) => boolean): RouteParams;
    getStorageParams(): StorageParams;
    getSecret(): string;
    getRootPassword(): string;
    isAuthRequired(): boolean;
}

export interface RouteParams {
    readonly apiRoute: string;
    readonly serviceRoute: string;
    readonly serviceHost: string;
    readonly authorize: boolean;
}

export interface StorageParams {
    readonly storage: string;
    readonly database: string;
}

export async function initSettings(path: string): Promise<Settings> {
    const query: Query = await loadQuery(path);

    function querySingle(queryPath: string, predicate?: Predicate): QueryResult | null {
        const results: QueryResult[] = query(queryPath, predicate);
        return results.length > 0 ? results[0] : null;
    }

    return {
        getRouteParams(apiUrl: string, predicate: (api: string) => boolean): RouteParams {
            const apiRoute: QueryResult | null = querySingle("services/*/routes/*/api", predicate);
            if (!apiRoute) {
                throw new Error(`No api route is found for the url: ${apiUrl}`);
            }

            const serviceRoute: QueryResult | null = querySingle(`${apiRoute.path}/../service`);
            if (!serviceRoute) {
                throw new Error(`No service route is found for the url ${apiUrl}`);
            }

            const serviceHost: QueryResult | null = querySingle(`${apiRoute.path}/../../../host`);
            if (!serviceHost) {
                throw new Error(`No service host is found for the url ${apiUrl}`);
            }

            const authorize: QueryResult | null = querySingle(`${apiRoute.path}/../../../authorize`);

            return {
                apiRoute: apiRoute.value,
                serviceRoute: serviceRoute.value,
                serviceHost: serviceHost.value,
                authorize: authorize !== null,
            };
        },

        getStorageParams(): StorageParams {
            const storage: QueryResult | null = querySingle("authentication/storage");
            if (!storage) {
                throw new Error("gateway storage is not specified");
            }

            const database: QueryResult | null = querySingle("authentication/database");
            if (!database) {
                throw new Error("gateway database is not specified");
            }

            return {
                storage: storage.value,
                database: database.value,
            };
        },

        getSecret(): string {
            const secret: QueryResult | null = querySingle("authentication/secret");
            if (!secret) {
                throw new Error("No Authentication secret is specified for the gateway");
            }
            return secret.value;
        },

        getRootPassword(): string {
            const rootPassword: QueryResult | null = querySingle("authentication/root_password");
            if (!rootPassword) {
                throw new Error("No Root password is specified for the gateway");
            }
            return rootPassword.value;
        },

        isAuthRequired(): boolean {
            return querySingle("authentication") !== null;
        },
    };
}

async function loadQuery(settingsPath: string): Promise<Query> {
    const settingsYaml: string = await readFileAsync(settingsPath, "utf8");
    const settingsJson = yaml.safeLoad(settingsYaml);
    const { query, errors } = settingsParser(settingsJson);
    if (errors) {
        const errorMessage = errors.reduce((acc: string, err: ParseError) =>
            `${acc}\n${err.message} - ${err.path}`,
            "Wrong gateway settings format:");
        throw new Error(errorMessage);
    }
    return query!;
}

const settingsParser = json([
    prop("services", obj([
        propAny(obj([
            propOptional("authorize", value("boolean")),
            prop("host", value("string")),
            prop("routes", array(obj([
                prop("api", value("string")),
                prop("service", value("string")),
            ]))),
        ])),
    ])),
    propOptional("authentication", obj([
        prop("secret", value("string")),
        prop("storage", value("string")),
        prop("database", value("string")),
        prop("root_password", value("string")),
    ])),
]);

const readFileAsync = util.promisify(fs.readFile);
