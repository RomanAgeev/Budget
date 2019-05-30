import { NextFunction, Request, Response } from "express";
import * as jwt from "jsonwebtoken";
import { SettingsProvider, Settings } from "./settings";

const bearerPrefix = "Bearer ";

export const authHandler = (settingsProvider: SettingsProvider) =>
    async (req: Request, res: Response, next: NextFunction): Promise<void> => {
        const settings: Settings = await settingsProvider();

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
            next();
            return;
        }

        const secret: string = settings.getSecret();

        jwt.verify(token, secret, (err, tokenDecoded) => {
            if (err) {
                next(err);
                return;
            }

            (req as any).tokenDecoded = tokenDecoded;
            next();
        });
    };
