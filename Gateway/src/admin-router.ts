import { Router, Request, Response } from "express";
import { Storage } from "./storage";
import { UserModel, UserUpdateModel, userViewModel, rootname } from "./user-model";
import { okResult } from "./utils";
import { NextFunction } from "connect";
import { body, validationResult } from "express-validator/check";
import { validationError, domainError } from "./error-handler";

export const adminRouter = (storage: Storage) =>
    Router()
        .get("/users", getUsers(storage))

        .put("/users/:username", [
            body("enabled").exists().isBoolean(),
            body("admin").exists().isBoolean(),
        ], putUser(storage))

        .delete("/users/:username", deleteUser(storage));

export const getUsers = (storage: Storage) => async (req: Request, res: Response) => {
    const users: UserModel[] = await storage.getUsers();

    okResult(res, users.map(userViewModel));
};

export const putUser = (storage: Storage) => async (req: Request, res: Response, next: NextFunction) => {
    const validationErrors = validationResult(req);
    if (!validationErrors.isEmpty()) {
        next(validationError(validationErrors.array()));
        return;
    }

    const username: string = (req.params as any).username;

    if (username === rootname) {
        next(domainError("RootUserUpdateOrDelete", "it is forbidden to edit root admin"));
        return;
    }

    const updateModel: UserUpdateModel = req.body;

    const success = await storage.updateUser(username, updateModel);
    if (!success) {
        next(new Error(`failed to update ${username} user`));
        return;
    }

    const user: UserModel | null = await storage.getUser(username);
    if (!user) {
        next(new Error(`failed to get an updated user ${username}`))
        return;
    }

    okResult(res, userViewModel(user));
};

export const deleteUser = (storage: Storage) => async (req: Request, res: Response, next: NextFunction) => {
    const username: string = (req.params as any).username;

    if (username === rootname) {
        next(domainError("RootUserUpdateOrDelete", "it is forbidden to delete root admin"));
        return;
    }

    const success = await storage.deleteUser(username);
    if (!success) {
        next(new Error(`failed to delete ${username} user`));
        return;
    }

    okResult(res);
};
