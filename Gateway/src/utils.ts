import { Response } from "express";

export const invalidCredentialsMessage = "invalid username or password";

export const okResult = (res: Response, data?: any) => res.status(200).send(data);
export const badRequest = (res: Response, message: string) => res.status(400).send(message);
export const invalidCredentials = (res: Response) => badRequest(res, invalidCredentialsMessage);

const env = process.env.NODE_ENV || "development";

export const isDevelopment = (): boolean => env === "development";
export const isStaging = (): boolean => env === "staging";
export const isProduction = (): boolean => env === "production";
