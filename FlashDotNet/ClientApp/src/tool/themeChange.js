import {useDark, useToggle} from '@vueuse/core';
import * as languageChange from "@/tool/languageChange.js";
import {Moon, Sunny, Sunrise} from "@element-plus/icons-vue";

// 定义枚举
export const Theme = {
    LIGHT: 'light',
    DARK: 'dark',
    AUTO: 'auto',
};

// 获取主题
export function getTheme() {
    return localStorage.getItem('vueuse-color-scheme');
}

// 获取主题名称
export function getThemeIcon() {
    let theme = getTheme();
    switch (theme) {
        case Theme.LIGHT:
            return Sunny;
        case Theme.DARK:
            return Moon;
        case Theme.AUTO:
            return Sunrise;
        default:
    }
}

// 设置主题
export function setTheme(theme) {
    localStorage.setItem('vueuse-color-scheme', theme);
    const isDark = useDark();
    useToggle(isDark);
}

// 切换主题
export function toggleTheme() {
    let theme = getTheme();
    switch (theme) {
        case Theme.DARK:
            setTheme(Theme.LIGHT);
            break;
        case Theme.LIGHT:
            setTheme(Theme.AUTO);
            break;
        case Theme.AUTO:
        default:
            setTheme(Theme.DARK);
            break;
    }
}
