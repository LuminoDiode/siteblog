export default class envHelper{
    public static isDevelopment = () => process.env.REACT_APP_Environment==="Development";
    public static isProduction = () => process.env.REACT_APP_Environment==="Production";
}