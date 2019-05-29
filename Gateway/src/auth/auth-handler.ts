import { NextFunction, Request, Response } from "express";
import * as jwt from "jsonwebtoken";
import { SettingsProvider } from "../settings";
import { Query as Settings } from "@ra/json-queries";
import { authRequired, getAuthSecret } from "./auth-helper";

const bearerPrefix = "Bearer ";

export const authHandler = (settingsProvider: SettingsProvider) =>
    async (req: Request, res: Response, next: NextFunction): Promise<void> => {
        const settings: Settings = await settingsProvider();

        if (!authRequired(settings)) {
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

        const secret: string = getAuthSecret(settings);

        jwt.verify(token, secret, (err, tokenDecoded) => {
            if (err) {
                next(err);
                return;
            }

            (req as any).tokenDecoded = tokenDecoded;
            next();
        });
    };
