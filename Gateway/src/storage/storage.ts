import { MongoClient } from "mongodb";
import { Settings } from "../settings";
import { UserModel, UserUpdateModel } from "./user-model";

export interface Storage {
    getUsers(): Promise<UserModel[]>;
    getUser(username: string): Promise<UserModel | null>;
    addUser(user: UserModel): Promise<void>;
    updateUser(username: string, user: UserUpdateModel): Promise<void>;
    deleteUser(username: string): Promise<void>;
    close(): Promise<void>;
}

export async function initStorage(settings: Settings): Promise<Storage> {
    const { storage, database } = settings.getStorageParams();

    const client = await new MongoClient(storage).connect();
    const db = client.db(database);
    const collection = db.collection("users");

    return {
        async getUsers(): Promise<UserModel[]> {
            return await collection.find().toArray();
        },

        async getUser(username: string): Promise<UserModel | null> {
            const users = await collection.find({ username }).toArray();
            return users.length > 0 ? users[0] : null;
        },

        async addUser(user: UserModel): Promise<void> {
            await collection.insertOne(user);
        },

        async updateUser(username: string, updateModel: UserUpdateModel): Promise<void> {
            const { enabled, admin } = updateModel;
            await collection.replaceOne({ username }, { $set: { enabled, admin } });
        },

        async deleteUser(username: string): Promise<void> {
            await collection.remove({ username });
        },

        async close(): Promise<void> {
            await client.close();
        },
    };
}
