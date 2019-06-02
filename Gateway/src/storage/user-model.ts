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
