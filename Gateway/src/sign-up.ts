import { Request, Response } from "express";
import { SettingsProvider } from "./settings";
import { StorageProvider, createUser, Storage, UserModel } from "./storage";

export const signUp = (settingsProvider: SettingsProvider, storageProvider: StorageProvider) => async (req: Request, res: Response) => {
    const username: string = req.body.username;
    const password: string = req.body.password;

    if (!username || !password) {
        res.send(400);
        return;
    }

    const storage: Storage = await storageProvider();

    try {
        let user: UserModel | null = await storage.getUser(username);
        if (user) {
            res.send(400);
            return;
        }

        user = createUser(username, password);

        await storage.addUser(user);

        // TODO: check insert result

        res.send(200);
    } finally {
        await storage.close();
    }
};
