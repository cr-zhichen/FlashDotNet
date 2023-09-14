<script setup>

import {useGoToHome} from "@/router/goToRouter.js";
import {ref} from "@vue/reactivity";
import {onMounted} from "vue";

const goToHome = useGoToHome();

</script>

<template>
    <div id="notFoundPage">
        <h1>{{ $t('notFoundPage.title') }}</h1>
        <el-button @click="goToHome">{{ $t('notFoundPage.goHomeButton') }}</el-button>
    </div>
</template>

<style scoped>

</style>