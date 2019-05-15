import express from "express";
import { Express, Request, Response } from "express";
import bodyParser from "body-parser";
import path from "path";
import { gatewayHandler } from "./gateway";

const app: Express = express();

app.use(bodyParser.urlencoded({
    extended: false,
}));

app.use(bodyParser.json());

app.use(gatewayHandler(path.resolve(__dirname, "../gateway.yaml")));

app.get("/", (req: Request, res: Response) => res.send("Hello Gateway !!!"));

const port: number = Number(process.env.PORT) || 3000;

app.listen(port, () => console.log(`Gateway is listening on port ${port}...`));
