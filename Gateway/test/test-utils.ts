import * as sinon from "sinon";

export function assertOk(response: any, data: any) {
    assertStatus(response.status, 200);
    assertSend(response.send, data);
}

export function assertServerError(response: any, data: any) {
    assertStatus(response.status, 500);
    assertSend(response.send, data);
}

export function assertBadRequest(response: any, data?: string) {
    assertStatus(response.status, 400);
    assertSend(response.send, data);
}

export const assertUnauthorized = (response: any) => assertStatus(response.sendStatus, 401);

export const assertForbidden = (response: any) => assertStatus(response.sendStatus, 403);

function assertStatus(statusCall: any, status: number) {
    sinon.assert.calledOnce(statusCall);
    sinon.assert.calledWith(statusCall, status);
}

function assertSend(sendCall: any, data?: any) {
    sinon.assert.calledOnce(sendCall);
    if (data) {
        sinon.assert.calledWith(sendCall, data);
    }
}
