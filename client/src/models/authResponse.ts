import humanNotification from "./humanNotification";

export default class authResponse {
    status?: number;
    humanNotification?: humanNotification;
    jwtToken?: string;
}