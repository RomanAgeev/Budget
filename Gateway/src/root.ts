import { Settings } from "./settings";
import { UserModel, Storage, createRoot, rootUsername } from "./storage";

export async function ensureRoot(settings: Settings, storage: Storage): Promise<void> {
    const root: UserModel | null = await storage.getUser(rootUsername);
    if (!root) {
        const rootPassword = settings.getRootPassword();
        const success = await storage.addUser(createRoot(rootPassword));
        if (!success) {
            throw new Error("failed to create a root user");
        }
    }
}
