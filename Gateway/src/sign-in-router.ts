import { Storage } from "./storage";
import { UserModel } from "./user-model";
import { createHash } from "./password";
import { Request, Response, Router } from "express";
import * as jwt from "jsonwebtoken";
import { body } from "express-validator/check";
import { TokenPayload } from "./token";
import { validationHandler } from "./validation-handler";
import { NextFunction } from "connect";
import { credentialsError } from "./error-handler";

export const signInRouter = (secret: string, storage: Storage) =>
    Router()
        .post("/", [
            body("username").exists().isString(),
            body("password").exists().isString(),
        ], validationHandler, signIn(secret, storage));

export const signIn = (secret: string, storage: Storage) =>
    async (req: Request, res: Response, next: NextFunction) => {
        const { username, password } = req.body;

        if (!username || !password) {
            next(new Error("username or password doesn't exist"));
            return;
        }

        const user: UserModel | null = await storage.getUser(username);
        if (!user) {
            next(credentialsError());
            return;
        }

        const hash = createHash(password, user.salt);

        if (hash !== user.hash) {
            next(credentialsError());
            return;
        }

        const payload: TokenPayload = {
            username,
            enabled: user.enabled,
            admin: user.admin,
        };

        const token = jwt.sign(payload, secret, { expiresIn: "24h" });
        res.json({ token });
    };
