import axios from "axios";
import { Request, Response, NextFunction } from "express";
import { routeMatcher } from "./gateway-settings";

export const gatewayHandler = (settingsPath: string) => {
    const matchRoute = routeMatcher(settingsPath);

    return async (req: Request, res: Response, next: NextFunction): Promise<void> => {
        const privateUrl: string | null = await matchRoute(req.url);

        if (privateUrl) {
            const result = await request(privateUrl, req.method, req.body, e => {
                res.status(e.response.status).send(e.response.data);
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
