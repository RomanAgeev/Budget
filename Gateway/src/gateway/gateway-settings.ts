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
    visitText(value: string): boolean;
}

export class Validator implements IVisitor {
    public visitProp(name: string, obj: any): boolean {
        return obj && obj[name];
    }
    public visitText(value: string): boolean {
        return typeof value === "string";
    }
}

type Check = (obj: any) => Reduce;
type Reduce = (visitor: IVisitor, propName?: string) => boolean;

const text = (value: any): Reduce =>
    (visitor: IVisitor) => visitor.visitText(value);

const list = (check: Check) => (value: any): Reduce => {
    const reducers: Reduce[] = (value && value instanceof Array) ? value.map(it => check(it)) : [];

    return (visitor: IVisitor) => reducers.every(reduce => reduce(visitor)); // TODO: The same
};

const map = (checks: Check[]) => (value: any): Reduce => {
    const reducers: Reduce[] = value ? checks.map(check => check(value)) : [];

    return (visitor: IVisitor) => reducers.every(reduce => reduce(visitor)); // TODO: The same
};

const prop = (name: string, checkValue: Check) => (obj: any): Reduce => {
    const names = name === "*" ? Object.getOwnPropertyNames(obj) : [name];
    const reducers: Array<{ name: string, reducer: Reduce }> = names.map(it => ({
        name: it,
        reducer: checkValue(obj[it]),
    }));

    return (visitor: IVisitor) => reducers.every(it => visitor.visitProp(it.name, obj) && it.reducer(visitor, it.name));
};

const parseSettings = map([
    prop("services", map([
        prop("*", map([
            prop("host", text),
            prop("routes", list(map([
                prop("public", text),
                prop("private", text),
            ]))),
        ])),
    ])),
]);
