import { MongoClient } from "mongodb";
import { SettingsProvider } from "../settings";
import { Query as Settings, QueryResult } from "@ra/json-queries";

export type StorageProvider = () => Promise<MongoClient>;

export const initStorage = (settingsProvider: SettingsProvider) => async (): Promise<MongoClient> => {
    const settings: Settings = await settingsProvider();

    const storageUris: QueryResult[] = settings("authentication/storage");
    if (storageUris.length === 0) {
        throw new Error("gateway storage is not specified");
    }

    const storageUri: string = storageUris[0].value;

    return new MongoClient(storageUri);
};
