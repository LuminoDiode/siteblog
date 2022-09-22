import axios from "axios";
import urlHelper from "./urlHelper";


export class authCredentials {
    email?: string;
    password?: string;
    useRegister?: boolean;
}
export class authResult {
    status?: number;
    humanNotification?: string;
    jwtToken?: string;
}

export enum notificationTypes {
    ok = "OK",
    info = "INFO",
    warning = "WARNING",
    error = "ERROR"
}

export default class apiHelper {
    private static parseAxiosAuthResponse = (axiosResponse: any) => {
        const result = new authResult();
        result.status = axiosResponse.status;
        result.humanNotification = axiosResponse.data["humanNotification"];
        result.jwtToken = axiosResponse.data["jwtToken"];
        
        return result;
    }

    public static auth = class {
        public static Login = async (claims: authCredentials) => {
            return apiHelper.parseAxiosAuthResponse(await axios.post(urlHelper.getAuthUrl(), claims));
        }
        public static Register = async (claims: authCredentials) => {
            return apiHelper.parseAxiosAuthResponse(await axios.put(urlHelper.getAuthUrl(), claims));
        }
        public static Validate = async (userJwt: string) => {
            return apiHelper.parseAxiosAuthResponse(await axios.get(urlHelper.getAuthUrl(), {
                headers: { "Authorization": ["Bearer", userJwt].join(' ') }
            }));
        }
        public static Delete = async (userJwt: string) => {
            return apiHelper.parseAxiosAuthResponse(await axios.delete(urlHelper.getAuthUrl(), {
                headers: { "Authorization": ["Bearer", userJwt].join(' ') }
            }));
        }
    }
}
