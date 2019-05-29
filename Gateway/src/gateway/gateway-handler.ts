import axios from "axios";
import { Request, Response, NextFunction } from "express";
import { SettingsProvider } from "../settings";
import { Query as Settings } from "@ra/json-queries";
import { getServiceUrl } from "./gateway-helper";

export const gatewayHandler = (settingsProvider: SettingsProvider) =>
    async (req: Request, res: Response, next: NextFunction): Promise<void> => {
        const settings: Settings = await settingsProvider();

        const apiUrl = req.url;

        let serviceParams: { serviceUrl: string, authorize: boolean } | null = null;
        try {
            serviceParams = getServiceUrl(apiUrl, settings);
        } catch (e) {
            next(e);
            return;
        }

        if (serviceParams.authorize) {
            const tokenDecoded = (req as any).tokenDecoded;
            if (!tokenDecoded) {
                next(new Error("Not authenticated"));
                return;
            }
        }

        let result: any = null;
        try {
            result = await axios.request({
                url: serviceParams!.serviceUrl,
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
