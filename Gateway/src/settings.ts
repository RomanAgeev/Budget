import fs from "fs";
import util from "util";
import yaml from "js-yaml";
import { Query as Settings, json, prop, obj, propAny, value, array, propOptional, ParseError } from "@ra/json-queries";

export type SettingsProvider = () => Promise<Settings>;

export const initSettings = (settingsPath: string) => async (): Promise<Settings> => {
    if (!settings) {
        settings = await loadSettings(settingsPath);
    }
    return settings;
};

let settings: Settings | null = null;

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
    ])),
]);

const loadSettings = async (settingsPath: string): Promise<Settings> => {
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
};

const readFileAsync = util.promisify(fs.readFile);

