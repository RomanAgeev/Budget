import fs from "fs";
import util from "util";
import yaml from "js-yaml";
import UrlPattern from "url-pattern";
import { JsonQuery, value, map, prop, array } from "@ra/json-queries";

let settings: any = null;

export const routeMatcher = (settingsPath: string) => async (publicRoute: string): Promise<string | null> => {
    await ensureSettings(settingsPath);
    // TODO: implement route matcher
    return null;
};

const ensureSettings = async (settingsPath: string): Promise<void> => {
    if (!settings) {
        const settingsYaml: string = await readFileAsync(settingsPath, "utf8");
        settings = yaml.safeLoad(settingsYaml);

        const jsonQuery = parseSettings(settings);
        if (jsonQuery instanceof JsonQuery) {
            const results = jsonQuery.findMany("services/*/routes/*/public");
            if (results.length > 0) {
                console.log("public:" + JSON.stringify(results[0]));

                const _private = jsonQuery.findMany(results[0].path + "/./private");
                console.log("private:" + JSON.stringify(_private));

                const host = jsonQuery.findMany(results[0].path + "/./././host");
                console.log("host:" + JSON.stringify(host));
            }
        }
    }
};

const readFileAsync = util.promisify(fs.readFile);

const parseSettings = map([
    prop("services", map([
        prop("*", map([
            prop("host", value("string")),
            prop("routes", array(map([
                prop("public", value("string")),
                prop("private", value("string")),
            ]))),
        ])),
    ])),
]);
