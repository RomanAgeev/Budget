import { describe, it, beforeEach, afterEach } from "mocha";
import * as sinon from "sinon";
import { signUp } from "../src/sign-up-router";
import { UserModel } from "../src/user-model";
import { expect } from "chai";
import { assertOk, assertDomainError, assertInternalError } from "./test-utils";

describe("sing up", () => {
    const username = "test_user";
    const password = "test_pass";

    let next: any;
    let response: any;

    beforeEach(() => {
        next = sinon.spy();
        response = {
            status: sinon.stub().callsFake(() => response),
            send: sinon.stub().callsFake(() => response),
            json: sinon.spy(),
        };
    });

    afterEach(() => {
        sinon.restore();
    });

    it("successful sing up", async () => {
        const storage: any = {
            getUser: sinon.stub().resolves(undefined),
            addUser: sinon.stub().resolves(true),
        };

        const request: any = { body: { username, password } };

        await signUp(storage)(request, response, next);

        sinon.assert.calledOnce(storage.addUser);

        assertOk(next, response);

        const newUser: UserModel = storage.addUser.firstCall.args[0];

        expect(newUser.username).equal(username);
        expect(newUser.hash).not.empty;
        expect(newUser.salt).not.empty;
        expect(newUser.admin).false;
        expect(newUser.enabled).false;
    });


    it("no username provided", async () => {
        const storage: any = {
            getUser: sinon.stub().resolves(undefined),
            addUser: sinon.stub().resolves(true),
        };

        const request: any = { body: { password } };

        await signUp(storage)(request, response, next);

        sinon.assert.notCalled(storage.addUser);

        assertInternalError(next);
    });

    it("no password provided", async () => {
        const storage: any = {
            getUser: sinon.stub().resolves(undefined),
            addUser: sinon.stub().resolves(true),
        };

        const request: any = { body: { username } };

        await signUp(storage)(request, response, next);

        sinon.assert.notCalled(storage.addUser);

        assertInternalError(next);
    });

    it("user already exists", async () => {
        const user: UserModel = {
            username,
            salt: "test_salt",
            hash: "test_hash",
            admin: false,
            enabled: true,
        };

        const storage: any = {
            getUser: sinon.stub().withArgs(username).resolves(user),
            addUser: sinon.stub().resolves(true),
        };

        const request: any = { body: { username, password } };

        await signUp(storage)(request, response, next);

        sinon.assert.notCalled(storage.addUser);

        assertDomainError(next, "UserAlreadyExists");
    });

    it("failed to add user", async () => {
        const storage: any = {
            getUser: sinon.stub().resolves(undefined),
            addUser: sinon.stub().resolves(false),
        };

        const request: any = { body: { username, password } };

        await signUp(storage)(request, response, next);

        assertInternalError(next);
    });
});
