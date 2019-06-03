import { NextFunction, Request, Response } from "express";
import * as jwt from "jsonwebtoken";
import { Settings } from "./settings";
import { unauthorized, forbidden } from "./utils";
import { TokenPayload } from "./token";

const bearerPrefix = "Bearer ";

export const authHandler = (settings: Settings, adminOnly: boolean) =>
    (req: Request, res: Response, next: NextFunction) => {
        if (!settings.isAuthRequired()) {
            next();
            return;
        }

        const headers: any = req.headers;

        let token: string = headers["x-access-token"] || headers["authorization"];
        if (token && token.startsWith(bearerPrefix)) {
            token = token.slice(bearerPrefix.length, token.length);
        }

        if (!token) {
            unauthorized(res);
            return;
        }

        const secret: string = settings.getSecret();

        jwt.verify(token, secret, (err, tokenDecoded) => {
            if (err) {
                unauthorized(res);
                return;
            }

            const payload = tokenDecoded as TokenPayload;

            if (adminOnly && !payload.admin) {
                forbidden(res);
                return;
            }

            (req as any).tokenDecoded = tokenDecoded;
            next();
        });
    };