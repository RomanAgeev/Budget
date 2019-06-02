import { Router, Request, Response } from "express";
import { Storage, UserModel, UserUpdateModel, userViewModel, UserViewModel } from "./storage";
import { badRequest, okResult } from "./utils";

export const admin = (storage: Storage) =>
    Router()
        .get("/users", async (req: Request, res: Response) => {
            const users: UserModel[] = await storage.getUsers();

            okResult(res, users.map(userViewModel));
        })
        .put("/users/:username", async (req: Request, res: Response) => {
            const username: string = (req.params as any).username;

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
        })
        .delete("/users/:username", async (req: Request, res: Response) => {
            const username: string = (req.params as any).username;

            const success = await storage.deleteUser(username);
            if (!success) {
                badRequest(res, `failed to delete ${username} user`);
            }

            okResult(res);
        });
