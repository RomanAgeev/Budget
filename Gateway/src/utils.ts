import { Response } from "express";

export const okResult = (res: Response, data?: any) => res.status(200).send(data);
export const badRequest = (res: Response, message: string) => res.status(400).send(message);
export const unauthorized = (res: Response) => res.sendStatus(401);
export const invalidCredentials = (res: Response) => badRequest(res, "invalid username or password");
