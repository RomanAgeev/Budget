import axios from "axios";
import { Request, Response, NextFunction } from "express";
import { routeMatcher } from "./gateway-settings";

export const gatewayHandler = (settingsPath: string) => {
    const matchRoute = routeMatcher(settingsPath);

    return async (req: Request, res: Response, next: NextFunction): Promise<void> => {
        let privateUrl: string | null = null;
        try {
            privateUrl = await matchRoute(req.url);
        } catch (e) {
            next(e);
            return;
        }

        if (privateUrl) {
            const result = await request(privateUrl, req.method, req.body, e => {
                if (e.response) {
                    res.status(e.response.status).send(e.response.data);
                } else {
                    next(new Error(`Service endpoint is unavailable : ${privateUrl}`));
                }
            });

            if (result) {
                res.send(result.data);
            }
        } else {
            next();
        }
    };
};

const request = async (url: string, method: string, data: any, handleError: (e: any) => void): Promise<any> => {
    try {
        return await axios.request({ url, method, data });
    } catch (e) {
        handleError(e);
        return null;
    }
};
