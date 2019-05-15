import express from "express";
import { Express, Request, Response, NextFunction } from "express";
import bodyParser from "body-parser";
import axios from "axios";
import UrlPattern from "url-pattern";

const app: Express = express();

app.use(bodyParser.urlencoded({
    extended: false,
}));

app.use(bodyParser.json());

interface IReroute {
    publicPattern: UrlPattern;
    privatePattern: UrlPattern;
}

const reroutes: IReroute[] = [
    {
        publicPattern: new UrlPattern("/api/category"),
        privatePattern: new UrlPattern("/api/v1/category"),
    },
    {
        publicPattern: new UrlPattern("/api/category/:categoryId"),
        privatePattern: new UrlPattern("/api/v1/category/:categoryId"),
    },
];

function gateway(req: Request, res: Response, next: NextFunction): void {
    const matches: Array<[IReroute, any]> = reroutes
        .map(it => [it, it.publicPattern.match(req.url)] as [IReroute, any])
        .filter(it => it[1] !== null);

    if (matches.length > 0) {
        const match: [IReroute, any] = matches[0];

        const privateUrl: string = "http://localhost:5000" + match[0].privatePattern.stringify(match[1]);

        (async () => {
            const config = {
                url: privateUrl,
                method: req.method,
                data: req.body,
            };

            let result: any = null;
            try {
                result = await axios.request({
                    ...config,
                });
            } catch (e) {
                res.status(e.response.status).send(e.response.data);
                return;
            }

            if (result) {
                res.send(result.data);
            }
        })();
    } else {
        next();
    }
}

app.use(gateway);

app.get("/", (req: Request, res: Response) => res.send("Hello Gateway !!!"));

const port: number = Number(process.env.PORT) || 3000;

app.listen(port, () => console.log(`Gateway is listening on port ${port}...`));
