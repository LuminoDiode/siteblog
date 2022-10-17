export enum userRoles {
    user = "user",
    moderator = "moderator",
    admin = "admin"
}
export default class jwtInfo {
    id?: string | number;
    email?: string;
    username?: string;
    role?: string | userRoles;
    nbf?: number;
    exp?: number;
    iat?: null;
}