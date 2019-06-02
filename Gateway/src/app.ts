import express from "express";
import { Express } from "express";
import { Server } from "http";
import bodyParser from "body-parser";
import path from "path";
import { gatewayHandler } from "./gateway-handler";
import { initStorage, Storage } from "./storage";
import { initSettings, Settings } from "./settings";
import { authHandler } from "./auth-handler";
import { signIn } from "./sign-in";
import { signUp } from "./sign-up";

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
    const storage: Storage = await initStorage(settings);

    app.post("/signin", signIn(settings, storage));
    app.post("/signup", signUp(storage));

    app.use(authHandler(settings));
    app.use(gatewayHandler(settings));

    const port: number = Number(process.env.PORT) || 3000;

    const server: Server = app.listen(port, () => console.log(`Gateway is listening on port ${port}...`));

    function gracefulExit() {
        console.log("Gateway is gracefully closing...");

        server.close(async () => {
            console.log("Gateway server is closed");

            await storage.close();
            console.log("Gateway storage is closed");

            process.exit(0);
        });
    }

    process.on("SIGINT", gracefulExit);
    process.on("SIGTERM", gracefulExit);
})();
