import fs from "fs";
import util from "util";
import yaml from "js-yaml";
import UrlPattern from "url-pattern";

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

        const settingsReducer = parseSettings(settings);
        if (!settingsReducer(new Validator())) {
            throw new Error("settings are wrong");
        }
    }
};

const readFileAsync = util.promisify(fs.readFile);

export interface IVisitor {
    visitProp(name: string, obj: any): boolean;
    visitText(value: string, propName?: string): boolean;
}

export class Validator implements IVisitor {
    public visitProp(name: string, obj: any): boolean {
        return obj && obj[name];
    }
    public visitText(value: string, propName: string): boolean {
        return typeof value === "string";
    }
}

type Check = (obj: any, propName?: string) => Reduce;
type Reduce = (visitor: IVisitor, propName?: string) => boolean;

const text = (value: any, propName?: string): Reduce =>
    (visitor: IVisitor) => visitor.visitText(value, propName);

const list = (check: Check) => (value: any, propName?: string): Reduce => {
    const reducers: Reduce[] = (value && value instanceof Array) ? value.map(it => check(it, propName)) : [];

    return (visitor: IVisitor) => reducers.every(reduce => reduce(visitor, propName)); // TODO: The same
};

const map = (checks: Check[]) => (value: any, propName?: string): Reduce => {
    const reducers: Reduce[] = value ? checks.map(check => check(value, propName)) : [];

    return (visitor: IVisitor) => reducers.every(reduce => reduce(visitor, propName)); // TODO: The same
};

const prop = (name: string, checkValue: Check) => (obj: any): Reduce => {
    const reducer: Reduce = checkValue(obj[name], name);

    return (visitor: IVisitor) => visitor.visitProp(name, obj) && reducer(visitor, name);
};

const parseSettings = map([
    prop("services", map([
        prop("expenses", map([ // TODO: implement general solution like prop("*", ...)
            prop("host", text),
            prop("routes", list(map([
                prop("public", text),
                prop("private", text),
            ]))),
        ])),
    ])),
]);
