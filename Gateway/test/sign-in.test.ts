import { describe, it, afterEach } from "mocha";
import { expect } from "chai";
import sinon from "sinon";
import jwt from "jsonwebtoken";
import { UserModel } from "../src/user-model";
import { Storage } from "../src/storage";
import { createHash } from "../src/password";
import { signIn } from "../src/sign-in";

describe("sign in", () => {
    afterEach(() => {
        sinon.restore();
    });

    it("first test", async () => {
        const salt = "testsalt";
        const password = "testpass";

        const hash = createHash(password, salt);

        const user: UserModel = {
            username: "testuser",
            salt,
            hash,
            admin: false,
            enabled: true,
        };

        const fakeStorage: Storage = {
            getUsers: sinon.fake(),
            getUser: sinon.fake(),
            addUser: sinon.fake(),
            updateUser: sinon.fake(),
            deleteUser: sinon.fake(),
            close: sinon.fake(),
        };

        sinon.replace(fakeStorage, "getUser", sinon.fake.resolves(user));

        const signin = signIn("testSecret", fakeStorage);

        const req: any = {
            body: {
                username: "testuser",
                password,
            },
        };

        const res: any = {
            status: sinon.fake(),
            sendStatus: sinon.fake(),
            send: sinon.fake(),
            json: sinon.fake(),
        };

        const stubJwtSign = sinon.stub(jwt, "sign").callsFake((payload, secret, options) => payload);

        await signin(req, res);

        sinon.assert.calledWith(res.json, {
            token: {
                username: "testuser",
                enabled: true,
                admin: false,
            },
        });
        sinon.assert.calledOnce(res.json);
        sinon.assert.calledOnce(stubJwtSign);
    });
});
