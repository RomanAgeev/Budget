import { NextFunction, Request, Response } from "express";
import * as jwt from "jsonwebtoken";
import { TokenPayload } from "./token";
import { unauthorized, forbidden } from "./error-handler";

const bearerPrefix = "Bearer ";

export const authHandler = (secret: string, adminOnly: boolean) =>
    (req: Request, res: Response, next: NextFunction) => {
        const headers: any = req.headers;

        let token: string = headers["x-access-token"] || headers["authorization"];
        if (token && token.startsWith(bearerPrefix)) {
            token = token.slice(bearerPrefix.length, token.length);
        } else {
            next(unauthorized());
            return;
        }

        jwt.verify(token, secret, (err, tokenDecoded) => {
            if (err) {
                next(unauthorized());
                return;
            }

            const payload = tokenDecoded as TokenPayload;

            if (!payload.enabled || (adminOnly && !payload.admin)) {
                next(forbidden());
                return;
            }

            (req as any).tokenDecoded = tokenDecoded;
            next();
        });
    };
