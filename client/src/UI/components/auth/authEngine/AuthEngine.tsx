import React, { Children,ReactDOM } from "react";
import apiHelper, { authCredentials } from "src/helpers/apiHelper";
import { credsInputGroupFuncs } from "../credentialsInputGroup/CredentialsInputGroup";


export interface ICredentialsProvider {
    submitCallback?: (creds: credsInputGroupFuncs) => void;
}

/**
 * DECORATOR PATTERN component. 
 * Accepts any ICredentialsProvider and uses proided credentials to auth user using remote server.
 * 
 * @param props 
 * @returns 
 */
export interface AuthEngineInterface {
    children: React.FC<ICredentialsProvider>
}

const AuthEngine : React.FC<AuthEngineInterface> = (props) => {
    const onSubmit = async (creds: credsInputGroupFuncs) => {
        const sendedInfo = new authCredentials();
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
                    creds.setAlerts([...creds.getAlerts!(), apiResponse.humanNotification!]);
                }
            }
        }
    }

    return props.children({submitCallback:onSubmit});
}

export default AuthEngine;