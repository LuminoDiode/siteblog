import authRequest from "src/models/authRequest";
import { credsInputGroupFuncs } from "../UI/components/auth/credentialsInputGroup/CredentialsInputGroup";
import apiHelper from "./apiHelper";

export class authHelper{
    static proceedAuth = async (creds: credsInputGroupFuncs) => {
        const sendedInfo = new authRequest();
        sendedInfo.email = creds.getEmail();
        sendedInfo.password = creds.getPassword();

        const apiResponse = creds.getIsRegister() ?
            await apiHelper.auth.Register(sendedInfo) : await apiHelper.auth.Login(sendedInfo);

        if (apiResponse.status && [200, 201].includes(apiResponse.status)) {
            if (creds.setAlerts) {
                creds.setAlerts([...creds.getAlerts!(), "Your are successfully logged in."]);
            }
        } else {
            if (apiResponse.humanNotification ?? false) {
                if (creds.setAlerts) {
                    creds.setAlerts([...creds.getAlerts!(), apiResponse.humanNotification?.message!]);
                }
            }
        }
    }
}