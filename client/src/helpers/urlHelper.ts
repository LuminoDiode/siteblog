import apiPaths from "../config/apiPaths.json"
import { urlJoin } from 'url-join-ts';

export default class urlHelper {
    public static getHostUrl = () => window.location.protocol + '//' + window.location.host;
    public static getAuthUrl = () => urlJoin(urlHelper.getHostUrl(), apiPaths.apiBasePath, apiPaths.authPath);
    public static getPostsInfoUrl = () => urlJoin(urlHelper.getHostUrl(), apiPaths.apiBasePath, apiPaths.postsInfoPath);
}
