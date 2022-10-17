import jwtDecode from "jwt-decode";
import jwtInfo from "src/models/jwtInfo";



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
