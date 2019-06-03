import { MongoClient } from "mongodb";
import { StorageSettings } from "./settings";
import { UserModel, UserUpdateModel, rootname, createRoot } from "./user-model";

export interface Storage {
    getUsers(): Promise<UserModel[]>;
    getUser(username: string): Promise<UserModel | null>;
    addUser(user: UserModel): Promise<boolean>;
    updateUser(username: string, user: UserUpdateModel): Promise<boolean>;
    deleteUser(username: string): Promise<boolean>;
    close(): Promise<void>;
}

export async function initStorage(settings: StorageSettings): Promise<Storage> {
    const server: string = settings.getServer();
    const database: string = settings.getDatabase();

    const storage: Storage = await openStorage(server, database);

    const root: UserModel | null = await storage.getUser(rootname);
    if (!root) {
        const rootpass = settings.getRootpass();
        const success = await storage.addUser(createRoot(rootpass));
        if (!success) {
            throw new Error("failed to create a root user");
        }
    }

    return storage;
}

async function openStorage(storage: string, database: string): Promise<Storage> {
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
