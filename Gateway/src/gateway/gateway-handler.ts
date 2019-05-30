import axios from "axios";
import { Request, Response, NextFunction } from "express";
import { SettingsProvider, Settings, RouteParams } from "../settings";
import UrlPattern from "url-pattern";

export const gatewayHandler = (settingsProvider: SettingsProvider) =>
    async (req: Request, res: Response, next: NextFunction): Promise<void> => {
        const settings: Settings = await settingsProvider();

        const apiUrl = req.url;

        let routeParams: RouteParams;
        try {
            routeParams = settings.getRouteParams(apiUrl, api => new UrlPattern(api).match(apiUrl) !== null);
        } catch (e) {
            next(e);
            return;
        }

        const { apiRoute, serviceRoute, serviceHost, authorize } = routeParams;

        if (authorize) {
            const tokenDecoded = (req as any).tokenDecoded;
            if (!tokenDecoded) {
                next(new Error("Not authenticated"));
                return;
            }
        }

        const apiPattern = new UrlPattern(apiRoute);
        const servicePattern = new UrlPattern(serviceRoute);
        const serviceUrl = serviceHost + servicePattern.stringify(apiPattern.match(apiUrl));

        let result: any = null;
        try {
            result = await axios.request({
                url: serviceUrl,
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
