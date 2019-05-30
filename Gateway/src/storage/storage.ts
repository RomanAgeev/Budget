import { MongoClient } from "mongodb";
import { SettingsProvider } from "../settings";
import { Query as Settings, QueryResult } from "@ra/json-queries";
import { UserModel } from "./user-model";

export interface Storage {
    getUser(username: string): Promise<UserModel | null>;
    addUser(user: UserModel): Promise<void>;
    close(): Promise<void>;
}

export type StorageProvider = () => Promise<Storage>;

export const initStorage = (settingsProvider: SettingsProvider): StorageProvider => async (): Promise<Storage> => {
    const settings: Settings = await settingsProvider();

    const storageUris: QueryResult[] = settings("authentication/storage");
    if (storageUris.length === 0) {
        throw new Error("gateway storage is not specified");
    }

    const storageUri: string = storageUris[0].value;

    const client = await new MongoClient(storageUri).connect();
    const db = client.db("budget_gateway_dev"); // TODO: get rid of constant database name
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
