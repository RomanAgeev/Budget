import { createSalt, createHash } from "./password";

export interface UserModel {
    username: string;
    hash: string;
    salt: string;
    enabled: boolean;
    admin: boolean;
}

export interface UserUpdateModel {
    enabled: boolean;
    admin: boolean;
}

export interface UserViewModel {
    username: string;
    enabled: boolean;
    admin: boolean;
}

export function userViewModel(user: UserModel): UserViewModel {
    return {
        username: user.username,
        enabled: user.enabled,
        admin: user.admin,
    };
}

export const createUser = (username: string, password: string): UserModel => createUserInternal(username, password, false, false);

function createUserInternal(username: string, password: string, enabled: boolean, admin: boolean): UserModel {
    const salt = createSalt();
    const hash = createHash(password, salt);

    return {
        username,
        hash,
        salt,
        enabled,
        admin,
    };
}
