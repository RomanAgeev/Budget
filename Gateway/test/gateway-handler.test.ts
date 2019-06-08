import { describe, it, beforeEach, afterEach } from "mocha";
import * as sinon from "sinon";
import axios from "axios";
import { RouteParams } from "../src/settings";
import { gatewayHandler } from "../src/gateway-handler";
import { assertOk, assertUnauthorized, assertDomainError, assertInternalError } from "./test-utils";

describe("gateway handler", () => {
    const url = "/test/api/1";
    const apiRoute = "/test/api(/:id)";
    const serviceRoute = "/test/service(/:id)";
    const serviceHost = "http://host";
    const serviceUrl = "http://host/test/service/1";
    const method = "post";
    const requestData = "test_data";
    const invalidRoute = "http://";

    let next: any;
    let response: any;

    beforeEach(() => {
        next = sinon.spy();
        response = {
            status: sinon.stub().callsFake(() => response),
            send: sinon.stub().callsFake(() => response),
            sendStatus: sinon.spy(),
            json: sinon.spy(),
        };
    });

    afterEach(() => {
        sinon.restore();
    });

    [
        false,
        true,
    ]
    .forEach(authorize => {
        it(`sucessful redirect ${authorize ? "and authorize" : ""}`, async () => {
            const routeParams: RouteParams = { apiRoute, serviceRoute, serviceHost, authorize };

            const settings: any = {
                getRouteParams: sinon.stub().withArgs(url).returns(routeParams),
            };

            const axiosStub = sinon.stub(axios, "request")
                .withArgs({
                    url: serviceUrl,
                    method,
                    data: requestData,
                })
                .resolves({
                    data: "test_response",
                });

            const request: any = {
                url,
                method,
                body: requestData,
                tokenDecoded: "decoded_token",
            };

            await gatewayHandler(settings)(request, response, next);

            sinon.assert.calledOnce(axiosStub);

            assertOk(next, response, "test_response");
        });
    });

    it("route not found", async () => {
        const settings: any = {
            getRouteParams: sinon.stub().withArgs(url).throws(new Error("not_found")),
        };

        const axiosStub = sinon.stub(axios, "request").resolves(undefined);

        const request: any = { url };

        await gatewayHandler(settings)(request, response, next);

        sinon.assert.notCalled(axiosStub);

        assertDomainError(next, "UnknownApiEndpoint");
    });

    it("invalid authorization token", async () => {
        const routeParams: RouteParams = { apiRoute, serviceRoute, serviceHost, authorize: true };

        const settings: any = {
            getRouteParams: sinon.stub().withArgs(url).returns(routeParams),
        };

        const axiosStub = sinon.stub(axios, "request").resolves(undefined);

        const request: any = { url };

        await gatewayHandler(settings)(request, response, next);

        sinon.assert.notCalled(axiosStub);

        assertUnauthorized(next);
    });

    it("invalid service request with status", async () => {
        const routeParams: RouteParams = { apiRoute, serviceRoute, serviceHost, authorize: false };

        const settings: any = {
            getRouteParams: sinon.stub().withArgs(url).returns(routeParams),
        };

        const rejectError: any = new Error("reject error");
        rejectError.response = {
            status: 500,
            data: "rejectData",
        };

        const axiosStub = sinon.stub(axios, "request")
            .withArgs({
                url: serviceUrl,
                method,
                data: requestData,
            })
            .rejects(rejectError);

        const request: any = { url, method, body: requestData };

        await gatewayHandler(settings)(request, response, next);

        sinon.assert.calledOnce(axiosStub);

        sinon.assert.notCalled(next);
        sinon.assert.calledOnce(response.status);
        sinon.assert.calledWith(response.status, 500);
        sinon.assert.calledOnce(response.send);
        sinon.assert.calledWith(response.send, "rejectData");
    });

    it("invalid service request without status", async () => {
        const routeParams: RouteParams = { apiRoute, serviceRoute, serviceHost, authorize: false };

        const settings: any = {
            getRouteParams: sinon.stub().withArgs(url).returns(routeParams),
        };

        const axiosStub = sinon.stub(axios, "request")
            .withArgs({
                url: serviceUrl,
                method,
                data: requestData,
            })
            .rejects(new Error("reject error"));

        const request: any = { url, method, body: requestData };

        await gatewayHandler(settings)(request, response, next);

        sinon.assert.calledOnce(axiosStub);

        assertDomainError(next, "UnavailableServiceEndpoint");
    });

    [
        [invalidRoute, serviceRoute],
        [apiRoute, invalidRoute],
    ]
    .forEach(([apiRt, serviceRt]) => {
        it(`invalid route (api: ${apiRt}), (service: ${serviceRt})`, async () => {
            const routeParams: RouteParams = {
                apiRoute: apiRt,
                serviceRoute: serviceRt,
                serviceHost,
                authorize: false,
            };

            const settings: any = {
                getRouteParams: sinon.stub().withArgs(url).returns(routeParams),
            };

            const axiosStub = sinon.stub(axios, "request").resolves(undefined);

            const request: any = { url };

            await gatewayHandler(settings)(request, response, next);

            sinon.assert.notCalled(axiosStub);

            assertInternalError(next);
        });
    });
});
