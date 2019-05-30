import express from "express";
import { Express } from "express";
import bodyParser from "body-parser";
import path from "path";
import { gatewayHandler } from "./gateway-handler";
import { initStorage } from "./storage";
import { initSettings } from "./settings";
import { authHandler } from "./auth-handler";
import { signIn } from "./sign-in";
import { signUp } from "./sign-up";

const settingsProvider = initSettings(path.resolve(__dirname, "../gateway.yaml"));
const storageProvider = initStorage(settingsProvider);

const app: Express = express();

app.use(bodyParser.urlencoded({
    extended: false,
}));

app.use(bodyParser.json());

app.post("/signin", signIn(settingsProvider, storageProvider));
app.post("/signup", signUp(settingsProvider, storageProvider));

app.use(authHandler(settingsProvider));
app.use(gatewayHandler(settingsProvider));

const port: number = Number(process.env.PORT) || 3000;

app.listen(port, () => console.log(`Gateway is listening on port ${port}...`));
