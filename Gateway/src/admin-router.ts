import { Router, Request, Response } from "express";
import { Storage } from "./storage";
import { UserModel, UserUpdateModel, userViewModel, rootname } from "./user-model";
import { badRequest, okResult } from "./utils";

export const adminRouter = (storage: Storage) =>
    Router()
        .get("/users", getUsers(storage))
        .put("/users/:username", putUser(storage))
        .delete("/users/:username", deleteUser(storage));

export const getUsers = (storage: Storage) => async (req: Request, res: Response) => {
    const users: UserModel[] = await storage.getUsers();

    okResult(res, users.map(userViewModel));
};

export const putUser = (storage: Storage) => async (req: Request, res: Response) => {
    const username: string = (req.params as any).username;

    if (username === rootname) {
        badRequest(res, "it is forbidden to edit root admin");
        return;
    }

    const updateModel: UserUpdateModel | undefined = (req as any).body.userUpdate;
    if (!updateModel) {
        badRequest(res, "no updateModel is specified");
        return;
    }

    const success = await storage.updateUser(username, updateModel);
    if (!success) {
        badRequest(res, `failed to update ${username} user`);
        return;
    }

    const user: UserModel | null = await storage.getUser(username);
    if (!user) {
        badRequest(res, `${username} user not found`);
        return;
    }

    okResult(res, userViewModel(user));
};

export const deleteUser = (storage: Storage) => async (req: Request, res: Response) => {
    const username: string = (req.params as any).username;

    if (username === rootname) {
        badRequest(res, "it is forbidden to delete root admin");
        return;
    }

    const success = await storage.deleteUser(username);
    if (!success) {
        badRequest(res, `failed to delete ${username} user`);
        return;
    }

    okResult(res);
};
