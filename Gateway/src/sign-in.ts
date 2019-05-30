import { StorageProvider, UserModel, validatePassword, Storage } from "./storage";
import { SettingsProvider, Settings } from "./settings";
import { Request, Response } from "express";
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
            const secret = settings.getSecret();

            const token = jwt.sign({ username }, secret, { expiresIn: "24h" });
            res.json({ token });
        } else {
            res.send(403);
        }
    } finally {
        await storage.close();
    }
};
