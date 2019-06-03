import { Storage } from "./storage";
import { UserModel } from "./user-model";
import { createHash } from "./password";
import { Request, Response } from "express";
import * as jwt from "jsonwebtoken";
import { invalidCredentials } from "./utils";
import { TokenPayload } from "./token";

export const signIn = (secret: string, storage: Storage) => async (req: Request, res: Response) => {
    const username: string = req.body.username;
    const password: string = req.body.password;

    if (!username || !password) {
        invalidCredentials(res);
        return;
    }

    const user: UserModel | null = await storage.getUser(username);
    if (!user) {
        invalidCredentials(res);
        return;
    }

    const hash = createHash(password, user.salt);

    if (hash !== user.hash) {
        invalidCredentials(res);
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
