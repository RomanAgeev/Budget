import express from "express";
import { Express, Request, Response } from "express";
import bodyParser from "body-parser";
import path from "path";
import { gatewayHandler } from "./gateway";
import { storageConnect } from "./storage";

const connectToStorage = storageConnect("mongodb://dev:dev@localhost:27017/budget_gateway_dev");

const app: Express = express();

app.use(bodyParser.urlencoded({
    extended: false,
}));

app.use(bodyParser.json());

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

app.use(gatewayHandler(path.resolve(__dirname, "../gateway.yaml")));

const port: number = Number(process.env.PORT) || 3000;

app.listen(port, () => console.log(`Gateway is listening on port ${port}...`));
