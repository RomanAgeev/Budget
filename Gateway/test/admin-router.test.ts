import { describe, it, beforeEach, afterEach } from "mocha";
import * as sinon from "sinon";
import { UserModel, UserUpdateModel, adminName } from "../src/user-model";
import { getUsers, putUser, deleteUser } from "../src/admin-router";
import { assertOk, assertDomainError, assertInternalError } from "./test-utils";

describe("admin-router", () => {
    const username1 = "test_user1";
    const username2 = "test_user2";

    const user1: UserModel = {
        username: username1,
        hash: "test_hash",
        salt: "test_salt",
        enabled: true,
        admin: false,
    };

    const user2: UserModel = {
        username: username2,
        hash: "test_hash",
        salt: "test_salt",
        enabled: true,
        admin: false,
    };

    const userUpdate: UserUpdateModel = {
        enabled: false,
        admin: true,
    };

    let response: any;
    let next: any;

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

    it("getUsers", async () => {
        const storage: any = {
            getUsers: () => [user1, user2],
        };

        const request: any = { };

        await getUsers(storage)(request, response);

        assertOk(next, response, [{
            username: username1,
            enabled: user1.enabled,
            admin: user1.admin,
        }, {
            username: username2,
            enabled: user2.enabled,
            admin: user2.admin,
        }]);
    });

    it("put user", async () => {
        const storage: any = {
            getUser: sinon.stub().withArgs(username1).resolves(user1),
            updateUser: sinon.stub().resolves(true),
        };

        const request: any = { params: { username: username1 }, body: userUpdate };

        await putUser(storage)(request, response, next);

        sinon.assert.calledOnce(storage.updateUser);
        sinon.assert.calledWith(storage.updateUser, username1, userUpdate);

        assertOk(next, response, {
            username: username1,
            enabled: user1.enabled,
            admin: user1.admin,
        });
    });

    it("no username provided", async () => {
        const storage: any = {
            updateUser: sinon.stub().resolves(true),
        };

        const request: any = { params: { }, body: userUpdate };

        await putUser(storage)(request, response, next);

        sinon.assert.notCalled(storage.updateUser);

        assertInternalError(next);
    });

    it("no update model provided", async () => {
        const storage: any = {
            updateUser: sinon.stub().resolves(true),
        };

        const request: any = { params: { username: username1 } };

        await putUser(storage)(request, response, next);

        sinon.assert.notCalled(storage.updateUser);

        assertInternalError(next);
    });

    it("attempt to change admin", async () => {
        const storage: any = {
            updateUser: sinon.stub().resolves(true),
        };

        const request: any = { params: { username: adminName }, body: userUpdate };

        await putUser(storage)(request, response, next);

        sinon.assert.notCalled(storage.updateUser);

        assertDomainError(next, "AdminUpdateOrDelete");
    });


    it("failed to update user", async () => {
        const storage: any = {
            updateUser: sinon.stub().resolves(false),
        };

        const request: any = { params: { username: username1 }, body: userUpdate };

        await putUser(storage)(request, response, next);

        sinon.assert.calledOnce(storage.updateUser);
        sinon.assert.calledWith(storage.updateUser, username1, userUpdate);

        assertInternalError(next);
    });

    it("failed to get user", async () => {
        const storage: any = {
            getUser: sinon.stub().withArgs(username1).resolves(undefined),
            updateUser: sinon.stub().resolves(true),
        };

        const request: any = { params: { username: username1 }, body: userUpdate };

        await putUser(storage)(request, response, next);

        sinon.assert.calledOnce(storage.updateUser);
        sinon.assert.calledWith(storage.updateUser, username1, userUpdate);

        assertInternalError(next);
    });

    it("delete user", async () => {
        const storage: any = {
            deleteUser: sinon.stub().resolves(true),
        };

        const request: any = { params: { username: username1 } };

        await deleteUser(storage)(request, response, next);

        sinon.assert.calledOnce(storage.deleteUser);
        sinon.assert.calledWith(storage.deleteUser, username1);

        assertOk(next, response);
    });

    it("delete user - no username provided", async () => {
        const storage: any = {
            deleteUser: sinon.stub().resolves(true),
        };

        const request: any = { params: { } };

        await deleteUser(storage)(request, response, next);

        sinon.assert.notCalled(storage.deleteUser);

        assertInternalError(next);
    });

    it("failed delete admin", async () => {
        const storage: any = {
            deleteUser: sinon.stub().resolves(true),
        };

        const request: any = { params: { username: adminName } };

        await deleteUser(storage)(request, response, next);

        sinon.assert.notCalled(storage.deleteUser);

        assertDomainError(next, "AdminUpdateOrDelete");
    });

    it("failed delete user", async () => {
        const storage: any = {
            deleteUser: sinon.stub().resolves(false),
        };

        const request: any = { params: { username: username1 } };

        await deleteUser(storage)(request, response, next);

        sinon.assert.calledOnce(storage.deleteUser);
        sinon.assert.calledWith(storage.deleteUser, username1);

        assertInternalError(next);
    });
});
