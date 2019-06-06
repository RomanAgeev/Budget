import axios, { Method } from "axios";
import { Request, Response, NextFunction } from "express";
import { Settings, RouteParams } from "./settings";
import * as UrlPattern from "url-pattern";
import { badRequest, okResult, unauthorized } from "./utils";

export const gatewayHandler = (settings: Settings) =>
    async (req: Request, res: Response, next: NextFunction) => {
        const apiUrl = req.url;

        let routeParams: RouteParams;
        try {
            routeParams = settings.getRouteParams(apiUrl, api => new UrlPattern(api).match(apiUrl) !== null);
        } catch (e) {
            badRequest(res, e.message);
            return;
        }

        const { apiRoute, serviceRoute, serviceHost, authorize } = routeParams;

        if (authorize) {
            const tokenDecoded = (req as any).tokenDecoded;
            if (!tokenDecoded) {
                unauthorized(res);
                return;
            }
        }

        let serviceUrl: string;
        try {
            const apiPattern = new UrlPattern(apiRoute);
            const servicePattern = new UrlPattern(serviceRoute);
            serviceUrl = serviceHost + servicePattern.stringify(apiPattern.match(apiUrl));
        } catch (e) {
            next(e);
            return;
        }

        const method = req.method as Method;

        let result: any;
        try {
            result = await axios.request({
                url: serviceUrl,
                method,
                data: req.body,
            });

        } catch (e) {
            if (e.response) {
                res.status(e.response.status).send(e.response.data);
            } else {
                badRequest(res, `Service endpoint is unavailable for api: ${apiUrl}`);
            }
            return;
        }

        okResult(res, result.data);
    };
