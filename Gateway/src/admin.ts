import { Router, Request, Response } from "express";
import { Storage, UserModel, UserUpdateModel, userViewModel, UserViewModel } from "./storage";

export const admin = (storage: Storage) =>
    Router()
        .get("/users", async (req: Request, res: Response) => {
            const users: UserModel[] = await storage.getUsers();
            const viewModels: UserViewModel[] = users.map(userViewModel);
            res.status(200).send(viewModels);
        })
        .put("/users/:username", async (req: Request, res: Response) => {
            const username: string = req.param("username");
            const updateModel: UserUpdateModel = (req as any).body.user.updateModel;

            await storage.updateUser(username, updateModel);

            const user: UserModel | null = await storage.getUser(username);
            if (!user) {
                res.send(400);
                return;
            }

            const viewModel: UserViewModel = userViewModel(user);

            res.status(200).send(viewModel);
        })
        .delete("/users/:username", async (req: Request, res: Response) => {
            const username: string = req.param("username");

            await storage.deleteUser(username);

            res.send(200);
        });
