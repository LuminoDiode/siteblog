import jwtDecode from "jwt-decode";


export enum userRoles {
    user = "user",
    moderator = "moderator",
    admin = "admin"
}
export class jwtInfo {
    id?: string | number;
    email?: string;
    username?: string;
    role?: string | userRoles;
    nbf?: number;
    exp?: number;
    iat?: null;
}

export default class jwtHelper {
    public static Validate = (jwt: string) => {
        if (!jwt) return false;

        var decodedJwt;
        try {
            decodedJwt = jwtDecode<jwtInfo>(jwt);
        } catch {
            return false;
        }
        if (decodedJwt.exp && (decodedJwt.exp! < Date.now())) return false;

        return true;        
    }

    public static Decode = (jwt:string) =>{
        return jwtDecode<jwtInfo>(jwt); 
    }
}
