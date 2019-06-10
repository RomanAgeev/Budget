import { MongoClient } from "mongodb";
import { StorageSettings } from "./settings";
import { UserModel, UserUpdateModel, createUser } from "./user-model";
import { Logger } from "pino";

export interface Storage {
    getUsers(): Promise<UserModel[]>;
    getUser(username: string): Promise<UserModel | null>;
    addUser(user: UserModel): Promise<boolean>;
    updateUser(username: string, user: UserUpdateModel): Promise<boolean>;
    deleteUser(username: string): Promise<boolean>;
    close(): Promise<void>;
}

export async function initStorage(settings: StorageSettings, logger: Logger): Promise<Storage> {
    const server: string = settings.getServer();
    const database: string = settings.getDatabase();
    const adminName: string = settings.getAdminName();

    const storage: Storage = await openStorage(server, database, logger);

    const admin: UserModel | null = await storage.getUser(adminName);
    if (!admin) {
        const adminPass = settings.getAdminPass();
        const success = await storage.addUser(createUser(adminName, adminPass));
        if (!success) {
            throw new Error("failed to create an admin");
        }
    }

    return storage;
}

async function openStorage(storage: string, database: string, logger: Logger): Promise<Storage> {
    const client = await new MongoClient(storage).connect();
    const db = client.db(database);
    const collection = db.collection("users");

    return {
        async getUsers(): Promise<UserModel[]> {
            logger.debug("db.users.findAll()");

            return await collection.find().toArray();
        },

        async getUser(username: string): Promise<UserModel | null> {
            logger.debug("db.users.find(%s)", username);

            const users = await collection.find({ username }).toArray();
            return users.length > 0 ? users[0] : null;
        },

        async addUser(user: UserModel): Promise<boolean> {
            logger.debug("db.users.insert(%o)", user);

            const op = await collection.insertOne(user);
            return op.result.n === 1;
        },

        async updateUser(username: string, updateModel: UserUpdateModel): Promise<boolean> {
            logger.debug("db.users.replace(%s, $o)", username, updateModel);

            const { enabled, admin } = updateModel;
            const op = await collection.replaceOne({ username }, { $set: { enabled, admin } });
            return op.result.n === 1;
        },

        async deleteUser(username: string): Promise<boolean> {
            logger.debug("db.users.delete(%s)", username);

            const op = await collection.deleteOne({ username });
            return op.result.n === 1;
        },

        async close(): Promise<void> {
            await client.close();
        },
    };
}
