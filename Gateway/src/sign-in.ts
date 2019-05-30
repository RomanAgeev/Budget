import { StorageProvider, UserModel, validatePassword } from "./storage";
import { MongoClient } from "mongodb";
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
    const storage: MongoClient = await storageProvider();

    await storage.connect();
    try {
        const db = storage.db("budget_gateway_dev"); // TODO: get rid of constant database name
        const usersCollection = db.collection("users");
        const users = await usersCollection.find({ username }).toArray();
        if (users.length === 0) {
            res.send(400);
            return;
        }

        const user: UserModel = users[0];

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
