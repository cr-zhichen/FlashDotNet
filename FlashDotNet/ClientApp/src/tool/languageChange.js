import {createI18n} from 'vue-i18n';
import zhCN from '@/locales/zh-CN.js';
import enUS from '@/locales/en-US.js';

//定义枚举
export const Language = {
    ZH_CN: '简体中文',
    EN_US: 'English',
}

// 获取用户首选语言
let userLanguage = localStorage.getItem('vueuse-lang') || navigator.language || navigator.userLanguage;

// 检查用户语言是否被支持
let supportedLanguages = ['zh-CN', 'en-US'];
if (!supportedLanguages.includes(userLanguage)) {
    userLanguage = 'en-US';  // 如果不支持用户的首选语言，设置一个默认语言
}

//将用户语言保存到 localStorage
localStorage.setItem('vueuse-lang', userLanguage);

// 创建 i18n 实例
export const i18n = createI18n({
    legacy: false,
    locale: userLanguage,
    messages: {
        'zh-CN': zhCN,
        'en-US': enUS
    }
});

// 获取语言名称
export function getLanguageName() {
    let lang = localStorage.getItem('vueuse-lang');
    switch (lang) {
        case 'zh-CN':
            return Language.ZH_CN;
        case 'en-US':
            return Language.EN_US;
        default:
    }
}

//设置语言
function setLanguage(lang) {
    localStorage.setItem('vueuse-lang', lang);
    i18n.global.locale.value = lang;
}

//切换语言
export function toggleLanguage() {
    let lang = localStorage.getItem('vueuse-lang');
    switch (lang) {
        case 'zh-CN':
            setLanguage('en-US');
            break;
        case 'en-US':
        default:
            setLanguage('zh-CN');
            break;
    }
}

// 获取全部语言
export function getLanguages() {
    let re = [];
    for (let lang in Language) {
        re.push({
            value: lang,
            label: Language[lang],
        });
    }
    return re;
}