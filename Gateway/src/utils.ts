import { Response } from "express";

export const invalidCredentialsMessage = "invalid username or password";

export const okResult = (res: Response, data?: any) => res.status(200).send(data);
export const badRequest = (res: Response, message: string) => res.status(400).send(message);
export const unauthorized = (res: Response) => res.sendStatus(401);
export const forbidden = (res: Response) => res.sendStatus(403);
export const invalidCredentials = (res: Response) => badRequest(res, invalidCredentialsMessage);
