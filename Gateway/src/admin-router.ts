import { Router, Request, Response } from "express";
import { Storage } from "./storage";
import { UserModel, UserUpdateModel, userViewModel } from "./user-model";
import { okResult } from "./utils";
import { NextFunction } from "connect";
import { body, param } from "express-validator/check";
import { domainError } from "./error-handler";
import { validationHandler } from "./validation-handler";

export const adminRouter = (storage: Storage, adminName: string) =>
    Router()
        .get("/users", getUsers(storage))

        .put("/users/:username", [
            param("username").exists().isString(),
            body("enabled").exists().isBoolean(),
            body("admin").exists().isBoolean(),
        ], validationHandler, putUser(storage, adminName))

        .delete("/users/:username", [
            param("username").exists().isString(),
        ], validationHandler, deleteUser(storage, adminName));

export const getUsers = (storage: Storage) =>
    async (req: Request, res: Response) => {
        const users: UserModel[] = await storage.getUsers();

        okResult(res, users.map(userViewModel));
    };

export const putUser = (storage: Storage, adminName: string) =>
    async (req: Request, res: Response, next: NextFunction) => {
        const username: string = (req.params as any).username;
        if (!username) {
            next(new Error("username doesn't exist"));
            return;
        }

        const updateModel: UserUpdateModel = req.body;
        if (!updateModel) {
            next(new Error("update model doesn't exist"));
            return;
        }

        if (username === adminName) {
            next(domainError("AdminUpdateOrDelete", "it is forbidden to edit admin"));
            return;
        }

        const success = await storage.updateUser(username, updateModel);
        if (!success) {
            next(new Error(`failed to update ${username} user`));
            return;
        }

        const user: UserModel | null = await storage.getUser(username);
        if (!user) {
            next(new Error(`failed to get an updated user ${username}`));
            return;
        }

        okResult(res, userViewModel(user));
    };

export const deleteUser = (storage: Storage, adminName: string) =>
    async (req: Request, res: Response, next: NextFunction) => {
        const username: string = (req.params as any).username;
        if (!username) {
            next(new Error("username doesn't exist"));
            return;
        }

        if (username === adminName) {
            next(domainError("AdminUpdateOrDelete", "it is forbidden to delete admin"));
            return;
        }

        const success = await storage.deleteUser(username);
        if (!success) {
            next(new Error(`failed to delete ${username} user`));
            return;
        }

        okResult(res);
    };
