import { MongoClient, ReplaceWriteOpResult, WriteOpResult } from "mongodb";
import { Settings } from "../settings";
import { UserModel, UserUpdateModel } from "./user-model";

export interface Storage {
    getUsers(): Promise<UserModel[]>;
    getUser(username: string): Promise<UserModel | null>;
    addUser(user: UserModel): Promise<boolean>;
    updateUser(username: string, user: UserUpdateModel): Promise<boolean>;
    deleteUser(username: string): Promise<boolean>;
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

        async addUser(user: UserModel): Promise<boolean> {
            const op = await collection.insertOne(user);
            return op.result.n === 1;
        },

        async updateUser(username: string, updateModel: UserUpdateModel): Promise<boolean> {
            const { enabled, admin } = updateModel;
            const op = await collection.replaceOne({ username }, { $set: { enabled, admin } });
            return op.result.n === 1;
        },

        async deleteUser(username: string): Promise<boolean> {
            const op = await collection.deleteOne({ username });
            return op.result.n === 1;
        },

        async close(): Promise<void> {
            await client.close();
        },
    };
}
