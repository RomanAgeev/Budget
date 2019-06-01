import express from "express";
import { Express } from "express";
import bodyParser from "body-parser";
import path from "path";
import { gatewayHandler } from "./gateway-handler";
import { initStorage, Storage } from "./storage";
import { initSettings, Settings } from "./settings";
import { authHandler } from "./auth-handler";
import { signIn } from "./sign-in";
import { signUp } from "./sign-up";

const app: Express = express();

app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

(async () => {
    const settings: Settings = await initSettings(path.resolve(__dirname, "../gateway.yaml"));
    const storage: Storage = await initStorage(settings);

    app.post("/signin", signIn(settings, storage));
    app.post("/signup", signUp(storage));

    app.use(authHandler(settings));
    app.use(gatewayHandler(settings));

    const port: number = Number(process.env.PORT) || 3000;

    app.listen(port, () => console.log(`Gateway is listening on port ${port}...`));
})();
