import fs from "fs";
import util from "util";
import yaml from "js-yaml";
import UrlPattern from "url-pattern";

let settings: any = null;

export const routeMatcher = (settingsPath: string) => async (publicRoute: string): Promise<string | null> => {
    await ensureSettings(settingsPath);
    // TODO: implement route matcher
    return null;
}

const ensureSettings = async (settingsPath: string): Promise<void> => {
    if (!settings) {
        const settingsYaml: string = await readFileAsync(settingsPath, "utf8");
        settings = yaml.safeLoad(settingsYaml);
    }
};

const readFileAsync = util.promisify(fs.readFile);
