<script setup>
import {useGoToNotFound} from "@/router/goToRouter.js";
import {ref} from "@vue/reactivity";
import {getThemeIcon, toggleTheme} from "@/tool/themeChange.js";
import {getLanguageName, toggleLanguage} from "@/tool/languageChange.js";

const theme = ref(getThemeIcon());
const swatchTheme = () => {
    toggleTheme();
    theme.value = getThemeIcon();
}

const language = ref(getLanguageName());
const switchLanguage = () => {
    toggleLanguage();
    language.value = getLanguageName();
}

const goToNotFound = useGoToNotFound();

</script>

<template>
    <div id="homePage">
        <h1>{{ $t('homePage.title') }}</h1>
        <el-button @click="swatchTheme" :icon="theme">{{ $t('homePage.swatchThemeButton') }}</el-button>
        <el-button @click="goToNotFound">{{ $t('homePage.notFoundButton') }}</el-button>
        <el-button @click="switchLanguage">{{ language }}</el-button>
    </div>
</template>

<style scoped>

</style>