export default class humanNotification{
    message?:string;
    type?:humanNotificationTypes
}
export enum humanNotificationTypes {
    ok = "OK",
    info = "INFO",
    warning = "WARNING",
    error = "ERROR"
}