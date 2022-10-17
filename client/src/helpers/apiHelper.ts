import axios from "axios";
import authRequest from "src/models/authRequest";
import authResponse from "src/models/authResponse";
import { postInfo } from "src/models/postInfo";
import postRequest from "src/models/postRequest";
import postsInfoRequest from "src/models/postsInfoRequest";
import urlHelper from "./urlHelper";


export default class apiHelper {
    public static auth = class {
        private static parseAxiosAuthResponse = (axiosResponse: any) => {
            const result = new authResponse();
            result.status = axiosResponse.status;
            result.humanNotification = axiosResponse.data["notification"];
            result.jwtToken = axiosResponse.data["jwtToken"];

            return result;
        }
        public static Login = async (claims: authRequest) => {
            return this.parseAxiosAuthResponse(await axios.post(urlHelper.getAuthUrl(), claims));
        }
        public static Register = async (claims: authRequest) => {
            return this.parseAxiosAuthResponse(await axios.put(urlHelper.getAuthUrl(), claims));
        }
        public static Validate = async (userJwt: string) => {
            return this.parseAxiosAuthResponse(await axios.get(urlHelper.getAuthUrl(), {
                headers: { "Authorization": ["Bearer", userJwt].join(' ') }
            }));
        }
        public static Delete = async (userJwt: string) => {
            return this.parseAxiosAuthResponse(await axios.delete(urlHelper.getAuthUrl(), {
                headers: { "Authorization": ["Bearer", userJwt].join(' ') }
            }));
        }
    }
    public static postsInfo = class {
        public static GetPostsInfos = async (query?: postsInfoRequest) => {
            return query ?
                (Array<postInfo>)(await axios.post(urlHelper.getPostsInfoUrl(), postsInfoRequest))
                :
                (Array<postInfo>)(await axios.get(urlHelper.getPostsInfoUrl()));
        }
        public static GetPostById= async (id:number):Promise<postInfo> => {
            return await axios.post(urlHelper.getPostsInfoUrl(), new postRequest(id));
        }
    }
}
