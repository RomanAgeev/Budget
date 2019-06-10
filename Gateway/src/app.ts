import * as express from "express";
import { Express } from "express";
import { Server } from "http";
import * as bodyParser from "body-parser";
import * as path from "path";
import * as pino from "pino";
import { Logger } from "pino";
import { gatewayHandler } from "./gateway-handler";
import { initStorage, Storage } from "./storage";
import { initSettings, Settings, StorageSettings } from "./settings";
import { authHandler } from "./auth-handler";
import { signInRouter } from "./sign-in-router";
import { singUpRouter } from "./sign-up-router";
import { adminRouter } from "./admin-router";
import { isDevelopment } from "./utils";
import { loggerHandler } from "./logger-handler";
import { errorHandler } from "./error-handler";

const logger: Logger = pino({
    level: isDevelopment() ? "debug" : "info",
    prettyPrint: true,
});

process.on("uncaughtException", pino.final(logger, (err, finalLogger) => {
    finalLogger.fatal(err, "uncaughtException");
    process.exit(1);
}));

(process as any).on("unhandledRejection", pino.final(logger, (err, finalLogger) => {
    finalLogger.fatal(err, "unhandledRejection");
    process.exit(1);
}));

(async () => {
    const app: Express = express();

    app.use(bodyParser.urlencoded({ extended: false }));
    app.use(bodyParser.json());
    app.use(loggerHandler(logger));

    const settings: Settings = await initSettings(path.resolve(__dirname, "../gateway.yaml"));

    let storage: Storage | null = null;

    const storageSettings: StorageSettings | null = settings.getStorageSettings();
    if (storageSettings) {
        storage = await initStorage(storageSettings, logger);

        const secret: string = storageSettings.getSecret();
        const adminName: string = storageSettings.getAdminName();

        app.use("/admin", authHandler(secret, true), adminRouter(storage, adminName));
        app.use("/signin", signInRouter(secret, storage));
        app.use("/signup", singUpRouter(storage));
        app.use(authHandler(secret, false));
    }

    app.use(gatewayHandler(settings));
    app.use(errorHandler(logger));

    const port: number = Number(process.env.PORT) || 3000;

    const server: Server = app.listen(port, () => logger.info(`Gateway is listening on port ${port}...`));

    function shutdown() {
        logger.info("Gateway is shutting down...");

        server.close(async () => {
            logger.info("Gateway server is closed");

            if (storage) {
                await storage.close();
                logger.info("Gateway storage is closed");
            }

            process.exit(0);
        });
    }

    process.on("SIGINT", shutdown);
    process.on("SIGTERM", shutdown);
})();
