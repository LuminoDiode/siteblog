import { group } from "console";
import { useEffect } from "react";
import { useState } from "react";
import classes from "./CredentialsInputGroup.module.css";


export class credsInputGroupInits {
    prompt?: string;
    buttonText?: string;
    alerts?: Array<string>;
    isRegister?: boolean;
    email?: string;
    password?: string;
}

export class credsInputGroupFuncs {
    public getEmail: () => string;
    public getPassword: () => string;
    public getPrompt: () => string;
    public getbuttonText: () => string;
    public getAlerts?: () => Array<string>;
    public getIsRegister: () => boolean;

    public setEmail: (str: string) => void;
    public setPassword: (str: string) => void
    public setPrompt: (str: string) => void;
    public setIsRegister: (bool: boolean) => void;
    public setButtonText: (str: string) => void;
    public setAlerts?: (alr: Array<string>) => void;

    constructor(
        getEmailFunc: () => string,
        getPasswordFunc: () => string,
        getPromptFunc: () => string,
        getbuttonTextFunc: () => string,
        getAlertsFunc: () => Array<string>,
        getIsRegisterFunc: () => boolean,

        setEmailFunc: (str: string) => void,
        setPasswordFunc: (str: string) => void,
        setPromptFunc: (str: string) => void,
        setButtonTextFunc: (str: string) => void,
        setAlertsFunc: (arr: Array<string>) => void,
        setIsRegisterFunc: (bool: boolean) => void,
    ) {
        this.getEmail = getEmailFunc;
        this.getPassword = getPasswordFunc;
        this.getPrompt = getPromptFunc;
        this.getbuttonText = getbuttonTextFunc;
        this.getAlerts = getAlertsFunc;
        this.getIsRegister = getIsRegisterFunc;

        this.setEmail = setEmailFunc;
        this.setPassword = setPasswordFunc;
        this.setPrompt = setPromptFunc;
        this.setIsRegister = setIsRegisterFunc;
        this.setButtonText = setButtonTextFunc;
        this.setAlerts = setAlertsFunc;
    }
}

/**
 * @param submitCallback Function to be called with entered credentials on sumbit button click.
 * @param groupParams Text which is showed before the inputs; Text in the submit button (default is "submit"); Alerts shown under the button.
 * @param props Props to be inserted into the root element of the component.
 * @returns Authentication credentials: email and password.
 */
const CredentialsInputGroup: React.FC<{
    submitCallback: (creds: credsInputGroupFuncs) => void,
    groupParams: credsInputGroupInits,
    props?: any[]
}> = (props) => {
    const [email, setEmail] = useState<string>(props.groupParams.email ?? '');
    const [password, setPassword] = useState<string>(props.groupParams.password ?? '');
    const [prompt, setPrompt] = useState<string>(props.groupParams.prompt ?? '');
    const [buttonText, setButtonText] = useState<string>(props.groupParams.buttonText ?? 'submit');
    const [alerts, setAlerts] = useState<Array<string>>(props.groupParams.alerts ?? []);
    const [isRegister, setIsRegister] = useState<boolean>(props.groupParams.isRegister ?? false);

    const onSubmit = () => {
        const funcs = new credsInputGroupFuncs(
            () => email,
            () => password,
            () => prompt,
            () => buttonText,
            () => alerts,
            () => isRegister,
            setEmail,
            setPassword,
            setPrompt,
            setButtonText,
            setAlerts,
            setIsRegister
        );
        props.submitCallback(funcs);
    }

    return (
        <span className={classes.loginGroup} {...props.props}>
            {prompt ?
                <span className={[classes.loginElement, classes.loginPrompt].join(' ')}>{prompt}</span>
                :
                <span />
            }

            <input className={[classes.loginElement, classes.loginInput].join(' ')}
                type={"text"} placeholder={"email"} value={email} onChange={e => setEmail(e.target.value)} />
            <input className={[classes.loginElement, classes.loginInput].join(' ')}
                type={"password"} placeholder={"password"} value={password} onChange={e => setPassword(e.target.value)} />
            <input className={[classes.loginElement, classes.loginInput, classes.loginButton].join(' ')}
                type={"button"} value={buttonText ?? "submit"} onClick={() => onSubmit()} />

            {alerts.length ?
                alerts.map(alert =>
                    <span className={[classes.loginElement, classes.loginAlert].join(' ')} style={{}}>{alert}</span>
                )
                :
                <span />
            }

            <span className={classes.loginElement} /> {/* zero margin-bottom fix. */}
        </span>
    );
}

export default CredentialsInputGroup;