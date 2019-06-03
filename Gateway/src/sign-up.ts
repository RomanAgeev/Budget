import { Request, Response } from "express";
import { Storage, UserModel, createUser } from "./storage";
import { createSalt, createHash } from "./password";
import { badRequest, okResult } from "./utils";

export const signUp = (storage: Storage) => async (req: Request, res: Response) => {
    const username: string = req.body.username;
    const password: string = req.body.password;

    if (!username || !password) {
        badRequest(res, "no username or password are specified");
        return;
    }

    const user: UserModel | null = await storage.getUser(username);
    if (user) {
        badRequest(res, `${username} user already exists`);
        return;
    }

    const newUser: UserModel = createUser(username, password);

    const success: boolean = await storage.addUser(newUser);
    if (!success) {
        badRequest(res, `failed to sign up ${username} user`);
        return;
    }

    okResult(res);
};
