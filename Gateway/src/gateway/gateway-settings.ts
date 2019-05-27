import fs from "fs";
import util from "util";
import yaml from "js-yaml";
import { Query as Settings, json, prop, obj, propAny, value, array, QueryResult, ParseError } from "@ra/json-queries";
import UrlPattern from "url-pattern";

const settingsParser = json([
    prop("services", obj([
        propAny(obj([
            prop("host", value("string")),
            prop("routes", array(obj([
                prop("api", value("string")),
                prop("service", value("string")),
            ]))),
        ])),
    ])),
]);

export const routeMatcher = (settingsPath: string) => {
    let settings: Settings | null = null;

    return async (apiUrl: string): Promise<string> => {
        if (!settings) {
            settings = await loadSettings(settingsPath);
        }

        const apiRoutes: QueryResult[] = settings!("services/*/routes/*/api", api => new UrlPattern(api).match(apiUrl) !== null);
        if (apiRoutes.length === 0) {
            throw new Error(`No api route is found for the url: ${apiUrl}`);
        }

        const apiRoute = apiRoutes[0];

        const serviceRoutes: QueryResult[] = settings!(`${apiRoute.path}/../service`);
        if (serviceRoutes.length === 0) {
            throw new Error(`No service route is found for the url ${apiUrl}`);
        }

        const serviceHosts: QueryResult[] = settings!(`${apiRoute.path}/../../../host`);
        if (serviceHosts.length === 0) {
            throw new Error(`No service host is found for the url ${apiUrl}`);
        }

        const serviceRoute = serviceRoutes[0];
        const serviceHost = serviceHosts[0];

        const apiPattern = new UrlPattern(apiRoute.value);
        const servicePattern = new UrlPattern(serviceRoute.value);
        return serviceHost.value + servicePattern.stringify(apiPattern.match(apiUrl));
    };
};

const loadSettings = async (settingsPath: string): Promise<Settings> => {
    const settingsYaml: string = await readFileAsync(settingsPath, "utf8");
    const settingsJson = yaml.safeLoad(settingsYaml);
    const { query, errors } = settingsParser(settingsJson);
    if (errors) {
        const errorMessage = errors.reduce((acc: string, err: ParseError) =>
            `${acc}\n${err.message} - ${err.path}`,
            "Wrong gateway format:");
        throw new Error(errorMessage);
    }
    return query!;
};

const readFileAsync = util.promisify(fs.readFile);
