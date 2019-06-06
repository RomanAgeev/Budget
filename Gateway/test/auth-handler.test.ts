import { describe, it, beforeEach, afterEach } from "mocha";
import * as sinon from "sinon";
import * as jwt from "jsonwebtoken";
import { authHandler } from "../src/auth-handler";
import { TokenPayload } from "../src/token";
import { expect } from "chai";
import { assertUnauthorized, assertForbidden } from "./test-utils";

describe("auth handler", () => {
    const secret = "test_secret";
    const token = "test_token";
    const username = "test_user";

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
        "x-access-token",
        "authorization",
    ]
    .forEach(header => {
        it(`decode ${header} token successfully`, async () => {
            const request: any = {
                headers: {
                    [header]: `Bearer ${token}`,
                },
            };

            const tokenDecoded: TokenPayload = {
                username,
                admin: false,
                enabled: true,
            };

            sinon.stub(jwt, "verify").withArgs(token, secret).callsArgWith(2, null, tokenDecoded);

            await authHandler(secret, false)(request, response, next);

            expect(request.tokenDecoded).equal(tokenDecoded);

            sinon.assert.calledOnce(next);
        });
    });

    it("no token provided", async () => {
        const request: any = { headers: { } };

        const jwtVerify = sinon.spy(jwt, "verify");

        await authHandler(secret, false)(request, response, next);

        sinon.assert.notCalled(jwtVerify);
        sinon.assert.notCalled(next);

        expect(response.tokenDecoded).undefined;

        assertUnauthorized(response);
    });

    it("invalid token format", async () => {
        const request: any = {
            headers: {
                authorization: `WRONG ${token}`,
            },
        };

        const jwtVerify = sinon.spy(jwt, "verify");

        await authHandler(secret, false)(request, response, next);

        sinon.assert.notCalled(jwtVerify);
        sinon.assert.notCalled(next);

        expect(response.tokenDecoded).undefined;

        assertUnauthorized(response);
    });

    it("token not verified", async () => {
        const request: any = {
            headers: {
                authorization: `Bearer ${token}`,
            },
        };

        sinon.stub(jwt, "verify").withArgs(token, secret).callsArgWith(2, new Error("not verified"), null);

        await authHandler(secret, false)(request, response, next);

        sinon.assert.notCalled(next);

        expect(response.tokenDecoded).undefined;

        assertUnauthorized(response);
    });

    it("user account disabled", async () => {
        const request: any = {
            headers: {
                authorization: `Bearer ${token}`,
            },
        };

        const tokenDecoded: TokenPayload = {
            username,
            admin: false,
            enabled: false,
        };

        sinon.stub(jwt, "verify").withArgs(token, secret).callsArgWith(2, null, tokenDecoded);

        await authHandler(secret, false)(request, response, next);

        sinon.assert.notCalled(next);

        expect(response.tokenDecoded).undefined;

        assertForbidden(response);
    });

    it("admin in adminonly mode", async () => {
        const request: any = {
            headers: {
                authorization: `Bearer ${token}`,
            },
        };

        const tokenDecoded: TokenPayload = {
            username,
            admin: true,
            enabled: true,
        };

        sinon.stub(jwt, "verify").withArgs(token, secret).callsArgWith(2, null, tokenDecoded);

        await authHandler(secret, true)(request, response, next);

        expect(request.tokenDecoded).equal(tokenDecoded);

        sinon.assert.calledOnce(next);
    });

    it("user in adminonly mode", async () => {
        const request: any = {
            headers: {
                authorization: `Bearer ${token}`,
            },
        };

        const tokenDecoded: TokenPayload = {
            username,
            admin: false,
            enabled: true,
        };

        sinon.stub(jwt, "verify").withArgs(token, secret).callsArgWith(2, null, tokenDecoded);

        await authHandler(secret, true)(request, response, next);

        sinon.assert.notCalled(next);

        expect(response.tokenDecoded).undefined;

        assertForbidden(response);
    });
});
