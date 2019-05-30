import { MongoClient } from "mongodb";
import { SettingsProvider, Settings } from "../settings";
import { UserModel } from "./user-model";

export interface Storage {
    getUser(username: string): Promise<UserModel | null>;
    addUser(user: UserModel): Promise<void>;
    close(): Promise<void>;
}

export type StorageProvider = () => Promise<Storage>;

export const initStorage = (settingsProvider: SettingsProvider): StorageProvider => async (): Promise<Storage> => {
    const settings: Settings = await settingsProvider();

    const { storage, database } = settings.getStorageParams();

    const client = await new MongoClient(storage).connect();
    const db = client.db(database);
    const collection = db.collection("users");

    return {
        async getUser(username: string): Promise<UserModel | null> {
            const users = await collection.find({ username }).toArray();
            return users.length > 0 ? users[0] : null;
        },

        async addUser(user: UserModel): Promise<void> {
            await collection.insertOne(user);
        },

        async close(): Promise<void> {
            await client.close();
        },
    };
};
