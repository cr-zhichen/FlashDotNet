<script setup>

import {globalState} from "./global/globalState.js";

</script>

<template>
    <div class="background"/>
    <router-view/>
</template>

<style scoped>
.background {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    /*放在最下层*/
    z-index: -1;
    background-image: radial-gradient(circle, rgba(58, 58, 58, 0.05) 2px, transparent 0);
    background-size: 30px 30px;
    background-position: 0 0, 15px 15px;
}

/*夜间模式*/
.dark .background {
    background-image: radial-gradient(circle, rgba(58, 58, 58, 0.20) 2px, transparent 0);
}

</style>
