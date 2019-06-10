import * as fs from "fs";
import * as util from "util";
import * as yaml from "js-yaml";
import { json, prop, obj, propAny, value, array, propOptional, ParseError, Query, QueryResult } from "@ra/json-queries";
import { Predicate } from "@ra/json-queries/dist/iterate";

export interface Settings {
    getRouteParams(apiUrl: string, predicate: (api: string) => boolean): RouteParams;
    getStorageSettings(): StorageSettings | null;
}

export interface StorageSettings {
    getServer(): string;
    getDatabase(): string;
    getSecret(): string;
    getAdminName(): string;
    getAdminPass(): string;
}

export interface RouteParams {
    readonly apiRoute: string;
    readonly serviceRoute: string;
    readonly serviceHost: string;
    readonly authorize: boolean;
}

const storagePath = "storage";

export async function initSettings(path: string): Promise<Settings> {
    const query: Query = await loadQuery(path);

    function querySingle(queryPath: string, predicate?: Predicate): QueryResult | null {
        const results: QueryResult[] = query(queryPath, predicate);
        return results.length > 0 ? results[0] : null;
    }

    const storageSettings: StorageSettings = {
        getServer(): string {
            const host: QueryResult | null = querySingle(`${storagePath}/host`);
            if (!host) {
                throw new Error("gateway storage host is not specified");
            }
            const user: QueryResult | null = querySingle(`${storagePath}/user`);
            if (!user) {
                throw new Error("gateway storage user is not specified");
            }
            const password: QueryResult | null = querySingle(`${storagePath}/password`);
            if (!password) {
                throw new Error("gateway storage password is not specified");
            }
            const database: QueryResult | null = querySingle(`${storagePath}/database`);
            if (!database) {
                throw new Error("gateway storage database is not specified");
            }

            return `mongodb://${user.value}:${password.value}@${host.value}/${database.value}`;
        },

        getDatabase(): string {
            const database: QueryResult | null = querySingle(`${storagePath}/database`);
            if (!database) {
                throw new Error("gateway storage database is not specified");
            }
            return database.value;
        },

        getSecret(): string {
            const secret: QueryResult | null = querySingle(`${storagePath}/secret`);
            if (!secret) {
                throw new Error("No authentication secret is specified for the gateway");
            }
            return secret.value;
        },

        getAdminName(): string {
            const adminName: QueryResult | null = querySingle(`${storagePath}/adminname`);
            if (!adminName) {
                throw new Error("No admin name is specified for the gateway");
            }
            return adminName.value;
        },

        getAdminPass(): string {
            const adminPassword: QueryResult | null = querySingle(`${storagePath}/adminpass`);
            if (!adminPassword) {
                throw new Error("No admin password is specified for the gateway");
            }
            return adminPassword.value;
        },
    };

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

        getStorageSettings(): StorageSettings | null {
            const storage: QueryResult | null = querySingle(storagePath);
            return storage !== null ? storageSettings : null;
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
    propOptional(storagePath, obj([
        prop("secret", value("string")),
        prop("host", value("string")),
        prop("user", value("string")),
        prop("password", value("string")),
        prop("database", value("string")),
        prop("adminname", value("string")),
        prop("adminpass", value("string")),
    ])),
]);

const readFileAsync = util.promisify(fs.readFile);
