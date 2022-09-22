import React from "react";
import cls from "./Header.module.css";
import LoginButton from '../../components/auth/LoginButton/LoginButton';
import { authHelper } from "src/helpers/authHelper";

const Header = () => {
    return (
        <header className={cls.appHeader}>
            <nav className={[cls.appHeaderRow, "verticalCenter"].join(' ')}>
                <span>
                    <span className={[cls.appHeaderTitle, cls.appHeaderPart, cls.appHeaderText].join(' ')}>
                        bruhcontent.ru
                    </span>
                    <span className={[cls.appHeaderPart, cls.appHeaderText].join(' ')}>
                        news
                    </span>
                    <span className={[cls.appHeaderPart, cls.appHeaderText].join(' ')}>
                        posts
                    </span>
                </span>
                <span className={[cls.appHeaderPart, cls.appHeaderText].join(' ')} style={{ minWidth: "15vw" }} >
                        <LoginButton submitCallback={authHelper.proceedAuth} />
                </span>
            </nav>
        </header>
    );
}

export default Header;