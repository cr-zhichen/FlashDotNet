import {globalState} from "../global/globalState.js";

//将Token存储到本地
export function setToken(token) {
    localStorage.setItem("token", token);
}

//从本地存储获取Token
export function getToken() {
    let token = localStorage.getItem("token");
    if (!token) {
        return null;
    } else {
        return token;
    }
}