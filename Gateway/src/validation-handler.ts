import { NextFunction, Request, Response } from "express";
import { validationResult } from "express-validator/check";
import { validationError } from "./error-handler";

export async function validationHandler(req: Request, res: Response, next: NextFunction) {
    const validationErrors = validationResult(req);
    if (validationErrors.isEmpty()) {
        next();
    } else {
        next(validationError(validationErrors.array()));
    }
}
