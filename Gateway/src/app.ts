import express from "express";
import { Express, Request, Response } from "express";
import bodyParser from "body-parser";


const app: Express = express();

app.use(bodyParser.urlencoded({
    extended: false,
}));

app.use(bodyParser.json());

app.get("/", (req: Request, res: Response) => res.send("Hello Gateway !!!"));

const port: number = Number(process.env.PORT) || 3000;

app.listen(port, () => console.log(`Gateway is listening on port ${port}...`));
