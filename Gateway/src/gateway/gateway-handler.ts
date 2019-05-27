import axios from "axios";
import { Request, Response, NextFunction } from "express";
import { routeMatcher } from "./gateway-settings";

export const gatewayHandler = (settingsPath: string) => {
    const matchRoute = routeMatcher(settingsPath);

    return async (req: Request, res: Response, next: NextFunction): Promise<void> => {
        const apiUrl = req.url;

        let serviceUrl: string | null = null;
        try {
            serviceUrl = await matchRoute(apiUrl);
        } catch (e) {
            next(e);
            return;
        }

        let result: any = null;
        try {
            result = await axios.request({
                url: serviceUrl!,
                method: req.method,
                data: req.body,
            });

        } catch (e) {
            if (e.response) {
                res.status(e.response.status).send(e.response.data);
            } else {
                next(new Error(`Service endpoint is unavailable for api: ${apiUrl}`));
            }
            return;
        }

        res.send(result.data);
    };
};
