import { Logger } from "pino";
import { NextFunction, Request, Response } from "express";

export const loggerHandler = (logger: Logger) =>
    (req: Request, res: Response, next: NextFunction) => {
        logger.info("%s %s %o", req.method, req.url, req.headers);
        next();
    };
