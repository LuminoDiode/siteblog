// import React, { useEffect, useState } from "react";
// import { useCookies } from "react-cookie";
// import jwtDecode from "jwt-decode";
// import apiHelper, { authCredentials } from "../../../../helpers/apiHelper"
// import jwtHelper, { jwtInfo } from "../../../../helpers/jwtHelper"
// import Modal from 'react-modal';
// import classes from "./LoginModal.module.css";
// import CredentialsInputGroup, { credsInputGroupFuncs, credsInputGroupInits } from "../credentialsInputGroup/CredentialsInputGroup";

// const LoginModal = (props: {
//     submitCallback: (creds: credsInputGroupFuncs) => void,
//     inputGroups?: Array<credsInputGroupInits>,
//     isOpen?:boolean,
//     modalStyles?: any,
//     props: any[]
// }) => {
//     const [isOpen, setIsOpen] = useState(props.isOpen ?? true);

//     const onSubmit = (creds: credsInputGroupFuncs) => {
//         props.submitCallback(creds);
//     }


//     return (
//         <Modal
//             isOpen={isOpen}
//             style={{ ...classes.loginModal, ...props.modalStyles }}
//             onRequestClose={() => setIsOpen(false)}
//             {...props.props}
//         >
//             {(props.inputGroups ?? [new credsInputGroupInits()]).map((gr: credsInputGroupInits) =>
//                 <CredentialsInputGroup submitCallback={(creds: credsInputGroupFuncs) => onSubmit(creds)} groupParams={gr} />
//             )}
//         </Modal>
//     );
// }

// export default LoginModal;