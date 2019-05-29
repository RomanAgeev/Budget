import { Query as Settings, QueryResult } from "@ra/json-queries";
import UrlPattern from "url-pattern";

export function getServiceUrl(apiUrl: string, settings: Settings): { serviceUrl: string, authorize: boolean } {
    const apiRoutes: QueryResult[] = settings("services/*/routes/*/api", api => new UrlPattern(api).match(apiUrl) !== null);
    if (apiRoutes.length === 0) {
        throw new Error(`No api route is found for the url: ${apiUrl}`);
    }

    const apiRoute = apiRoutes[0];

    const serviceRoutes: QueryResult[] = settings(`${apiRoute.path}/../service`);
    if (serviceRoutes.length === 0) {
        throw new Error(`No service route is found for the url ${apiUrl}`);
    }

    const serviceHosts: QueryResult[] = settings(`${apiRoute.path}/../../../host`);
    if (serviceHosts.length === 0) {
        throw new Error(`No service host is found for the url ${apiUrl}`);
    }

    const serviceRoute = serviceRoutes[0];
    const serviceHost = serviceHosts[0];

    const apiPattern = new UrlPattern(apiRoute.value);
    const servicePattern = new UrlPattern(serviceRoute.value);
    const serviceUrl = serviceHost.value + servicePattern.stringify(apiPattern.match(apiUrl));

    const authorizeResults: QueryResult[] = settings(`${apiRoute.path}/../../../authorize`);
    const authorize = authorizeResults.length > 0 && authorizeResults[0].value;

    return { serviceUrl, authorize };
}
