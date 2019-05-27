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

let settings: Settings | null = null;

export const routeMatcher = (settingsPath: string) => async (publicRoute: string): Promise<string | null> => {
    await ensureSettings(settingsPath);

    const apiResults: QueryResult[] = settings!("services/*/routes/*/api", api => new UrlPattern(api).match(publicRoute) !== null);
    if (apiResults.length === 1) {
        const serviceResults: QueryResult[] = settings!(`${apiResults[0].path}/../service`);
        if (serviceResults.length === 1) {
            const hostResults: QueryResult[] = settings!(`${apiResults[0].path}/../../../host`);
            if (hostResults.length === 1) {
                const apiPattern = new UrlPattern(apiResults[0].value);
                const servicePattern = new UrlPattern(serviceResults[0].value);
                const serviceUrl = servicePattern.stringify(apiPattern.match(publicRoute));
                return hostResults[0].value + serviceUrl;
            }
        }
    }

    return null;
};

const ensureSettings = async (settingsPath: string): Promise<void> => {
    if (!settings) {
        const settingsYaml: string = await readFileAsync(settingsPath, "utf8");
        const settingsJson = yaml.safeLoad(settingsYaml);
        const { query, errors } = settingsParser(settingsJson);
        if (errors) {
            const errorMessage = errors.reduce((acc: string, err: ParseError) =>
                `${acc}\n${err.message} - ${err.path}`,
                "Gateway format error:");
            throw new Error(errorMessage);
        }
        settings = query;
    }
};

const readFileAsync = util.promisify(fs.readFile);
