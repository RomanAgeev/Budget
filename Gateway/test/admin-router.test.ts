import { describe, it, beforeEach, afterEach } from "mocha";
import * as sinon from "sinon";
import { UserModel, UserViewModel, UserUpdateModel, rootname } from "../src/user-model";
import { getUsers, putUser, deleteUser } from "../src/admin-router";
import { assertOk, assertBadRequest } from "./test-utils";

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

    beforeEach(() => {
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

        assertOk(response, [{
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

        const request: any = { params: { username: username1 }, body: { userUpdate } };

        await putUser(storage)(request, response);

        sinon.assert.calledOnce(storage.updateUser);
        sinon.assert.calledWith(storage.updateUser, username1, userUpdate);

        assertOk(response, {
            username: username1,
            enabled: user1.enabled,
            admin: user1.admin,
        });
    });

    it("failed put root user", async () => {
        const storage: any = {
            updateUser: sinon.stub().resolves(true),
        };

        const request: any = { params: { username: rootname } };

        await putUser(storage)(request, response);

        sinon.assert.notCalled(storage.updateUser);

        assertBadRequest(response);
    });

    it("no update model provided", async () => {
        const storage: any = {
            updateUser: sinon.stub().resolves(true),
        };

        const request: any = { params: { username: username1 }, body: { } };

        await putUser(storage)(request, response);

        sinon.assert.notCalled(storage.updateUser);

        assertBadRequest(response);
    });

    it("failed to update user", async () => {
        const storage: any = {
            updateUser: sinon.stub().resolves(false),
        };

        const request: any = { params: { username: username1 }, body: { userUpdate } };

        await putUser(storage)(request, response);

        sinon.assert.calledOnce(storage.updateUser);
        sinon.assert.calledWith(storage.updateUser, username1, userUpdate);

        assertBadRequest(response);
    });

    it("failed to get user", async () => {
        const storage: any = {
            getUser: sinon.stub().withArgs(username1).resolves(undefined),
            updateUser: sinon.stub().resolves(true),
        };

        const request: any = { params: { username: username1 }, body: { userUpdate } };

        await putUser(storage)(request, response);

        sinon.assert.calledOnce(storage.updateUser);
        sinon.assert.calledWith(storage.updateUser, username1, userUpdate);

        assertBadRequest(response);
    });

    it("delete user", async () => {
        const storage: any = {
            deleteUser: sinon.stub().resolves(true),
        };

        const request: any = { params: { username: username1 } };

        await deleteUser(storage)(request, response);

        sinon.assert.calledOnce(storage.deleteUser);
        sinon.assert.calledWith(storage.deleteUser, username1);

        assertOk(response);
    });

    it("failed delete root user", async () => {
        const storage: any = {
            deleteUser: sinon.stub().resolves(true),
        };

        const request: any = { params: { username: rootname } };

        await deleteUser(storage)(request, response);

        sinon.assert.notCalled(storage.deleteUser);

        assertBadRequest(response);
    });

    it("failed delete user", async () => {
        const storage: any = {
            deleteUser: sinon.stub().resolves(false),
        };

        const request: any = { params: { username: username1 } };

        await deleteUser(storage)(request, response);

        sinon.assert.calledOnce(storage.deleteUser);
        sinon.assert.calledWith(storage.deleteUser, username1);

        assertBadRequest(response);
    });
});
