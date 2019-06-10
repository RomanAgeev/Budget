import { Logger } from "pino";
import { NextFunction, Request, Response } from "express";

export type ErrorType = "domain" | "unauthorized" | "forbidden";

export type ErrorCause =
    "AdminUpdateOrDelete" |
    "UserAlreadyExists" |
    "InvalidUserCredentials" |
    "UnknownApiEndpoint" |
    "UnavailableServiceEndpoint";

export interface GatewayError {
    type: ErrorType;
    cause: string;
    errors: any;
}

export const domainError = (cause: ErrorCause, message: string): GatewayError => ({
    type: "domain",
    cause,
    errors: [ message ],
});

export const validationError = (errors: any[]): GatewayError => ({
    type: "domain",
    cause: "Validation",
    errors,
});

export const credentialsError = (): GatewayError => ({
    type: "domain",
    cause: "InvalidUserCredentials",
    errors: "invalid username or password",
});

export const unauthorized = (): GatewayError => ({
    type: "unauthorized",
    cause: "Unauthorized",
    errors: "request is not authorized",
});

export const forbidden = (): GatewayError => ({
    type: "forbidden",
    cause: "Forbidden",
    errors: "request is forbiden",
});

export const errorHandler = (logger: Logger) =>
    (err: any, req: Request, res: Response, next: NextFunction) => {
        const status = getStatus(err);
        const cause: string = err.cause;
        const errors: any = err.errors || err.message;

        if (status === 500) {
            logger.error("%d %s %o", status, cause, errors);
        } else {
            logger.warn("%d %s %o", status, cause, errors);
        }

        res.status(status).send({ cause, errors });
    };

function getStatus(err: any): number {
    if (err.status) {
        return err.status;
    }

    const errorType: ErrorType | undefined = err.type;
    switch (errorType) {
        case "domain":
            return 400;
        case "unauthorized":
            return 401;
        case "forbidden":
            return 403;
        default:
            return 500;
    }
}
