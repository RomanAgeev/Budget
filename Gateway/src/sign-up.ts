import { Request, Response } from "express";
import { SettingsProvider } from "./settings";
import { StorageProvider, createUser } from "./storage";
import { MongoClient } from "mongodb";

export const signUp = (settingsProvider: SettingsProvider, storageProvider: StorageProvider) => async (req: Request, res: Response) => {
    const username: string = req.body.username;
    const password: string = req.body.password;

    if (!username || !password) {
        res.send(400);
        return;
    }

    const storage: MongoClient = await storageProvider();

    await storage.connect();
    try {
        const db = storage.db("budget_gateway_dev"); // TODO: get rid of constant database name
        const usersCollection = db.collection("users");
        const users = await usersCollection.find({ username }).toArray();
        if (users.length > 0) {
            res.send(400);
            return;
        }

        const user = createUser(username, password);

        await usersCollection.insertOne(user);

        // TODO: check insert result

        res.send(200);
    } finally {
        await storage.close();
    }
};
