import { StorageProvider, UserModel, validatePassword, Storage } from "./storage";
import { SettingsProvider } from "./settings";
import { Query as Settings } from "@ra/json-queries";
import { Request, Response } from "express";
import { getAuthSecret } from "./auth/auth-helper";
import * as jwt from "jsonwebtoken";

export const signIn = (settingsProvider: SettingsProvider, storageProvider: StorageProvider) => async (req: Request, res: Response) => {
    const username: string = req.body.username;
    const password: string = req.body.password;

    if (!username || !password) {
        res.send(400);
        return;
    }

    const settings: Settings = await settingsProvider();
    const storage: Storage = await storageProvider();

    try {
        const user: UserModel | null = await storage.getUser(username);
        if (!user) {
            res.send(400);
            return;
        }

        if (validatePassword(password, user)) {
            const secret = getAuthSecret(settings);

            const token = jwt.sign({ username }, secret, { expiresIn: "24h" });
            res.json({ token });
        } else {
            res.send(403);
        }
    } finally {
        await storage.close();
    }
};
