import * as crypto from "crypto";

export const createSalt = (): string => crypto.randomBytes(16).toString("hex");
export const createHash = (password: string, salt: string): string => crypto.pbkdf2Sync(password, salt, 1000, 64, "sha512").toString("hex");
