import { describe, it, beforeEach, afterEach } from "mocha";
import * as sinon from "sinon";
import * as jwt from "jsonwebtoken";
import { UserModel } from "../src/user-model";
import { createHash } from "../src/password";
import { signIn } from "../src/sign-in";
import { invalidCredentialsMessage } from "../src/utils";

describe("sign in", () => {
    const secret = "test_secret";
    const salt = "test_salt";
    const username = "test_user";
    const password = "test_pass";

    const hash = createHash(password, salt);

    const user: UserModel = {
        username,
        salt,
        hash,
        admin: false,
        enabled: true,
    };

    let response: any;

    function assertInvalidCredentials() {
        sinon.assert.calledOnce(response.status);
        sinon.assert.calledOnce(response.send);

        sinon.assert.calledWith(response.status, 400);
        sinon.assert.calledWith(response.send, invalidCredentialsMessage);
    }

    beforeEach(() => {
        response = {
            status: sinon.stub().callsFake(() => response),
            send: sinon.stub().callsFake(() => response),
            json: sinon.spy(),
        };
    });

    afterEach(() => {
        sinon.restore();
    });

    it("successful sign in", async () => {
        const storage: any = {
            getUser: sinon.stub().withArgs(username).resolves(user),
        };

        const jwtSign = sinon.stub(jwt, "sign").returnsArg(0);

        const request: any = { body: { username, password } };

        await signIn(secret, storage)(request, response);

        sinon.assert.calledOnce(response.json);
        sinon.assert.calledOnce(jwtSign);

        sinon.assert.calledWith(response.json, {
            token: {
                username,
                admin: false,
                enabled: true,
            },
        });
    });

    it("no username provided", async () => {
        const storage: any = { };

        const request: any = { body: { password } };

        await signIn(secret, storage)(request, response);

        assertInvalidCredentials();
    });

    it("no password provided", async () => {
        const storage: any = { };

        const request: any = { body: { username } };

        await signIn(secret, storage)(request, response);

        assertInvalidCredentials();
    });

    it("invalid username", async () => {
        const wrongname = "wrongname";

        const getUser = sinon.stub();
        getUser.withArgs(username).resolves(user);
        getUser.withArgs(wrongname).resolves(undefined);

        const storage: any = { getUser };

        const request: any = { body: { username: wrongname, password } };

        await signIn(secret, storage)(request, response);

        assertInvalidCredentials();
    });

    it("invalid password", async () => {
        const storage: any = {
            getUser: sinon.stub().withArgs(username).resolves(user),
        };

        const request: any = { body: { username, password: "wrongpass" } };

        await signIn(secret, storage)(request, response);

        assertInvalidCredentials();
    });
});
