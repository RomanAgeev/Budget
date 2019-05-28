import { MongoClient } from "mongodb";

export const storageConnect = (connectionString: string) => async (): Promise<MongoClient> => new MongoClient(connectionString).connect();
