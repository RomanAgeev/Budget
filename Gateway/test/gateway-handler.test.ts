import { describe, it, beforeEach, afterEach } from "mocha";
import * as sinon from "sinon";
import axios from "axios";
import { RouteParams } from "../src/settings";
import { gatewayHandler } from "../src/gateway-handler";
import { assertOk, assertServerError, assertBadRequest, assertUnauthorized } from "./test-utils";

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
            sinon.assert.notCalled(next);

            assertOk(response, "test_response");
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
        sinon.assert.notCalled(next);

        assertBadRequest(response, "not_found");
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
        sinon.assert.notCalled(next);

        assertUnauthorized(response);
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

        assertServerError(response, "rejectData");
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

        sinon.assert.notCalled(next);
        sinon.assert.calledOnce(axiosStub);

        assertBadRequest(response);
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
            sinon.assert.calledOnce(next);
        });
    });
});
