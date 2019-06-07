import * as express from "express";
import { Express } from "express";
import { Server } from "http";
import * as bodyParser from "body-parser";
import * as path from "path";
import { gatewayHandler } from "./gateway-handler";
import { initStorage, Storage } from "./storage";
import { initSettings, Settings, StorageSettings } from "./settings";
import { authHandler } from "./auth-handler";
import { signIn } from "./sign-in";
import { signUp } from "./sign-up";
import { adminRouter } from "./admin-router";

// tslint:disable: no-console

process.on("unhandledRejection", (err: any) => {
    console.error("An unhandled error occured on gateway start");
    console.error(err);
    process.exit(1);
});

(async () => {
    const app: Express = express();

    app.use(bodyParser.urlencoded({ extended: false }));
    app.use(bodyParser.json());

    const settings: Settings = await initSettings(path.resolve(__dirname, "../gateway.yaml"));

    let storage: Storage | null = null;

    const storageSettings: StorageSettings | null = settings.getStorageSettings();
    if (storageSettings) {
        storage = await initStorage(storageSettings);

        const secret: string = storageSettings.getSecret();

        app.use("/admin", authHandler(secret, true), adminRouter(storage));
        app.post("/signin", signIn(secret, storage));
        app.post("/signup", signUp(storage));
        app.use(authHandler(secret, false));
    }

    app.use(gatewayHandler(settings));

    const port: number = Number(process.env.PORT) || 3000;

    const server: Server = app.listen(port, () => console.log(`Gateway is listening on port ${port}...`));

    function shutdown() {
        console.log("Gateway is shutting down...");

        server.close(async () => {
            console.log("Gateway server is closed");

            if (storage) {
                await storage.close();
                console.log("Gateway storage is closed");
            }

            process.exit(0);
        });
    }

    process.on("SIGINT", shutdown);
    process.on("SIGTERM", shutdown);
})();
