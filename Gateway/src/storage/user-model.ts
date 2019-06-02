import * as crypto from "crypto";

export interface UserModel {
    username: string;
    email: string;
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

export function createUser(username: string, email: string, password: string): UserModel {
    const salt = createSalt();
    const hash = createHash(password, salt);

    return {
        username,
        email,
        hash,
        salt,
        enabled: false,
        admin: false,
    };
}

export function validatePassword(password: string, user: UserModel): boolean {
    const hash = createHash(password, user.salt);
    return hash === user.hash;
}

const createSalt = (): string => crypto.randomBytes(16).toString("hex");
const createHash = (password: string, salt: string): string => crypto.pbkdf2Sync(password, salt, 1000, 64, "sha512").toString("hex");
