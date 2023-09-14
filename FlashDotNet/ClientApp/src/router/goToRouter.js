import {useRouter} from "vue-router";
import {id} from "element-plus/es/locale/index";

//跳转到首页
export function useGoToHome() {
    const router = useRouter();

    return function () {
        router.push("/");
    };
}

//跳转到404页面
export function useGoToNotFound() {
    const router = useRouter();

    return function () {
        router.push("/NotFound");
    };
}