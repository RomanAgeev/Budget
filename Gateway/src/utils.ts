import { Response } from "express";

export const okResult = (res: Response, data?: any) => res.status(200).send(data);

const env = process.env.NODE_ENV || "development";

export const isDevelopment = (): boolean => env === "development";
export const isStaging = (): boolean => env === "staging";
export const isProduction = (): boolean => env === "production";
