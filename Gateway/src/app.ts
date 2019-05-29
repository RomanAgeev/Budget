import express from "express";
import { Express, Request, Response } from "express";
import bodyParser from "body-parser";
import path from "path";
import * as jwt from "jsonwebtoken";
import { gatewayHandler } from "./gateway";
import { storageConnect } from "./storage";
import { initSettings } from "./settings";
import { authHandler } from "./auth";
import { Query as Settings } from "@ra/json-queries";
import { getAuthSecret } from "./auth/auth-helper";

const settingsProvider = initSettings(path.resolve(__dirname, "../gateway.yaml"));

const connectToStorage = storageConnect("mongodb://dev:dev@localhost:27017/budget_gateway_dev");

const app: Express = express();

app.use(bodyParser.urlencoded({
    extended: false,
}));

app.use(bodyParser.json());

app.post("/login", async (req: Request, res: Response) => {
    const settings: Settings = await settingsProvider();

    const username: string = req.body.username;
    const password: string = req.body.password;

    // Read from database
    const fakeUsername = "test_user";
    const fakePassword = "test_password";

    const secret = getAuthSecret(settings);

    if (username && password) {
        if (username === fakeUsername && password === fakePassword) {
            const token = jwt.sign({ username }, secret, { expiresIn: "24h" });

            res.json({
                token,
            });
        } else {
            res.send(403); // Fobidden
        }
    } else {
        res.send(400); // Bad Request
    }
});

app.get("/auth", async (req: Request, res: Response) => {
    try {
        const storageClient = await connectToStorage();

        const db = storageClient.db("budget_gateway_dev");
        const users = db.collection("users");

        const userDocs = await users.find({}).toArray();

        await storageClient.close();

        res.send(JSON.stringify(userDocs));
    } catch (e) {
        console.error(e.message);
    }
});

app.use(authHandler(settingsProvider));
app.use(gatewayHandler(settingsProvider));

const port: number = Number(process.env.PORT) || 3000;

app.listen(port, () => console.log(`Gateway is listening on port ${port}...`));
