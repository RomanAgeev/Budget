import { Settings } from "./settings";
import { UserModel, Storage } from "./storage";
import { createSalt, createHash } from "./password";

export const rootAdminName = "__root__";

export async function ensureRootAdmin(settings: Settings, storage: Storage): Promise<void> {
    const rootAdmin: UserModel | null = await storage.getUser(rootAdminName);
    if (!rootAdmin) {
        const rootPassword = settings.getRootPassword();

        const salt = createSalt();
        const hash = createHash(rootPassword, salt);

        const newRootAdmin: UserModel = {
            username: rootAdminName,
            hash,
            salt,
            enabled: true,
            admin: true,
        };

        await storage.addUser(newRootAdmin);
    }
}
