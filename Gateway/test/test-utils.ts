import * as sinon from "sinon";
import { ErrorCause } from "../src/error-handler";

export function assertDomainError(next: any, cause: ErrorCause) {
    sinon.assert.calledOnce(next);
    sinon.assert.calledWith(next, sinon.match({
        type: "domain",
        cause,
    }));
}

export function assertUnauthorized(next: any) {
    sinon.assert.calledOnce(next);
    sinon.assert.calledWith(next, sinon.match({
        type: "unauthorized",
        cause: "Unauthorized",
    }));
}

export function assertForbidden(next: any) {
    sinon.assert.calledOnce(next);
    sinon.assert.calledWith(next, sinon.match({
        type: "forbidden",
        cause: "Forbidden",
    }));
}

export function assertCredentialsError(next: any) {
    sinon.assert.calledOnce(next);
    sinon.assert.calledWith(next, sinon.match({
        type: "domain",
        cause: "InvalidUserCredentials",
    }));
}

export function assertInternalError(next: any) {
    sinon.assert.calledOnce(next);
    sinon.assert.calledWith(next, sinon.match.instanceOf(Error));
}

export function assertOk(next: any, response: any, data?: any) {
    sinon.assert.notCalled(next);
    sinon.assert.calledOnce(response.status);
    sinon.assert.calledWith(response.status, 200);
    sinon.assert.calledOnce(response.send);
    if (data) {
        sinon.assert.calledWith(response.send, data);
    }
}
