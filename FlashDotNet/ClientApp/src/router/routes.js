const routes = [
    {
        path: '/',
        name: 'home',
        title: '首页',
        component: () => import('@/page/HomePage.vue'), //.vue不能省略
    },
    //其他路由跳转到NotFound
    {
        path: '/:pathMatch(.*)',
        name: 'notFound',
        title: '404',
        component: () => import('@/page/NotFoundPage.vue'), //.vue不能省略
    }
]
export default routes
