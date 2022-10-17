import React from "react";
import cls from "./Header.module.css";
import LoginButton from '../../components/auth/LoginButton/LoginButton';
import { authHelper } from "src/helpers/authHelper";
import { NavLink } from "react-router-dom";

const Header = () => {
    return (
        <header className={cls.appHeader}>
            <nav className={[cls.appHeaderRow, "verticalCenter"].join(' ')}>
                <span>
                    <NavLink to="/" className={[cls.appHeaderTitle, cls.appHeaderPart, cls.appHeaderText].join(' ')}>
                        bruhcontent.ru
                    </NavLink>
                    <NavLink to="/news" className={[cls.appHeaderPart, cls.appHeaderText].join(' ')}>
                        news
                    </NavLink>
                    <NavLink to="/blog" className={[cls.appHeaderPart, cls.appHeaderText].join(' ')}>
                        blog
                    </NavLink>
                    <NavLink to="/services" className={[cls.appHeaderPart, cls.appHeaderText].join(' ')}>
                        services
                    </NavLink>
                </span>
                <span  className={[cls.appHeaderPart, cls.appHeaderText].join(' ')} style={{ minWidth: "15vw" }} >
                        <LoginButton submitCallback={authHelper.proceedAuth} />
                </span>
            </nav>
        </header>
    );
}

export default Header;