import * as crypto from "crypto";

export interface UserModel {
    username: string;
    hash: string;
    salt: string;
}

export function createUser(username: string, password: string): UserModel {
    const salt = createSalt();
    const hash = createHash(password, salt);

    return {
        username,
        hash,
        salt,
    };
}

export function validatePassword(password: string, user: UserModel): boolean {
    const hash = createHash(password, user.salt);
    return hash === user.hash;
}

const createSalt = (): string => crypto.randomBytes(16).toString("hex");
const createHash = (password: string, salt: string): string => crypto.pbkdf2Sync(password, salt, 1000, 64, "sha512").toString("hex");
