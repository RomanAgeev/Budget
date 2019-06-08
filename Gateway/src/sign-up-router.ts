import { Request, Response, Router } from "express";
import { Storage } from "./storage";
import { UserModel, createUser } from "./user-model";
import { okResult } from "./utils";
import { NextFunction } from "connect";
import { body } from "express-validator/check";
import { validationHandler } from "./validation-handler";
import { domainError } from "./error-handler";

export const singUpRouter = (storage: Storage) =>
    Router()
        .post("/", [
            body("username").exists().isString(),
            body("password").exists().isString(),
        ], validationHandler, signUp(storage));

export const signUp = (storage: Storage) =>
    async (req: Request, res: Response, next: NextFunction) => {
        const { username, password } = req.body;

        if (!username || !password) {
            next(new Error("username or password doesn't exist"));
            return;
        }

        const user: UserModel | null = await storage.getUser(username);
        if (user) {
            next(domainError("UserAlreadyExists", `${username} user already exists`));
            return;
        }

        const newUser: UserModel = createUser(username, password);

        const success: boolean = await storage.addUser(newUser);
        if (!success) {
            next(new Error(`failed to sign up ${username} user`));
            return;
        }

        okResult(res);
    };
