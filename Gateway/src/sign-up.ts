import { Request, Response } from "express";
import { Storage, UserModel, createSalt, createHash } from "./storage";
import { badRequest, okResult } from "./utils";

export const signUp = (storage: Storage) => async (req: Request, res: Response) => {
    const email: string = req.body.email;
    const username: string = req.body.username || email;
    const password: string = req.body.password;

    if (!email || !username || !password) {
        badRequest(res, "no username or password are specified");
        return;
    }

    const user: UserModel | null = await storage.getUser(username);
    if (user) {
        badRequest(res, `${username} user already exists`);
        return;
    }

    const salt = createSalt();
    const hash = createHash(password, salt);

    const newUser: UserModel = {
        username,
        email,
        hash,
        salt,
        enabled: false,
        admin: false,
    };

    const success: boolean = await storage.addUser(newUser);
    if (!success) {
        badRequest(res, `failed to sign up ${username} user`);
        return;
    }

    okResult(res);
};
