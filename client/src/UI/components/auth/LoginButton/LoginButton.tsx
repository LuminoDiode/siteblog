import React, { useEffect, useState } from "react";
import { useCookies } from "react-cookie";
import Modal from "react-modal"
import { authHelper } from "src/helpers/authHelper";
import jwtInfo from "src/models/jwtInfo";


import apiHelper from "../../../../helpers/apiHelper"
import jwtHelper from "../../../../helpers/jwtHelper"

import CredentialsInputGroup, { credsInputGroupFuncs, credsInputGroupInits } from '../credentialsInputGroup/CredentialsInputGroup';

import classes from "./LoginButton.module.css";

export interface LoginButtonProps {
    submitCallback?: (creds: credsInputGroupFuncs) => void;
    inputGroups?: Array<credsInputGroupInits>,
    replaceWithCredential?: boolean,
    buttonText?: string,
    props?: any[]
}

const LoginButton: React.FC<LoginButtonProps> = (props) => {
    const [getCookie, setCookie, removeCookie] = useCookies(["jwt"]);
    const [isAuthed, setIsAuthed] = useState(false);
    const [isLoginModalOpen, setLoginModalOpen] = useState(false);
    const [currentCredentials, setCurrentCredentials] = useState<jwtInfo>();
    const [inpGroups, setInpGroups] = useState<Array<credsInputGroupInits>>(props?.inputGroups ?? []);

    const onLoad = useEffect(() => {
        if (jwtHelper.Validate(getCookie['jwt'])) {
            setCurrentCredentials(jwtHelper.Decode(getCookie["jwt"]));
            setIsAuthed(true);
        }
        if (!inpGroups.length) {
            const loginGroup = new credsInputGroupInits();
            loginGroup.prompt = "Login";
            loginGroup.isRegister = false;

            const registerGroup = new credsInputGroupInits();
            registerGroup.prompt = "Register";
            loginGroup.isRegister = true;

            setInpGroups([loginGroup, registerGroup]);
        }
    });

    const getDisplayedName = () => {
        if (currentCredentials?.username ?? false) {
            return currentCredentials!.username;
        } else if (currentCredentials?.email ?? false) {
            return currentCredentials!.email;
        } else if (currentCredentials?.id ?? false) {
            return currentCredentials!.id;
        } else {
            return "unknown";
        }
    }

    const logout = () => {
        removeCookie("jwt");
        setIsAuthed(false);
    }

    const submit = (creds: credsInputGroupFuncs) => {
        console.log('submittin...');
        if (props?.submitCallback) {
            props.submitCallback(creds);
        } else {
            authHelper.proceedAuth(creds);
        }
    }


    return (
        <span style={{ width: "100%", height: "100%" }} {...props}>
            {(isAuthed && props?.replaceWithCredential) ?
                <span>
                    <span>
                        {getDisplayedName()}
                    </span >
                    <span style={{ display: 'flex', justifyContent: 'center' }}>
                        <button className={classes.loginBtn} onClick={() => logout()}>Logout</button>
                    </span>
                </span>
                :
                <span>
                    <button className={classes.loginButton} onClick={() => setLoginModalOpen(true)} >
                        {props?.buttonText ? props.buttonText : "login"}
                    </button>
                    <span>
                        <Modal
                            isOpen={isLoginModalOpen}
                            className={classes.loginModal}
                            onRequestClose={() => setLoginModalOpen(false)}
                        >
                            {(inpGroups ?? [new credsInputGroupInits()]).map((gr, index) =>
                                <span key={index}>
                                    {index > 0 ?
                                        <span style={{ marginTop: "2vh", display: "block" }} />
                                        :
                                        <span />
                                    }
                                    <CredentialsInputGroup submitCallback={creds => submit(creds)} groupParams={gr} />
                                </span>
                            )}
                        </Modal>
                    </span>
                </span>
            }
        </span>
    );
}

export default LoginButton;