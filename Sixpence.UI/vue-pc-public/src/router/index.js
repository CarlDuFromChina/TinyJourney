import Vue from 'vue';
import App from '../App.vue';
import VueRouter from 'vue-router';
import NProgress from 'nprogress';
import 'nprogress/nprogress.css';

Vue.use(VueRouter);

const router = new VueRouter({
  mode: 'history',
  routes: [
    {
      // 顶层
      path: '/',
      component: App,
      children: [
        {
          path: '/index',
          name: 'index',
          component: () => import('@/views/index/index.vue'),
          redirect: '/index/home',
          children: [
            {
              path: '/index/categories',
              name: 'categories',
              component: () => import('@/views/index/categories.vue'),
              meta: { title: '文章分类', keepAlive: true },
            },
            {
              path: '/index/home',
              name: 'home',
              component: () => import('@/views/index/home/index.vue'),
              props: true,
              meta: { title: '主页', keepAlive: true }
            },
            {
              path: '/index/guidelines',
              name: 'guidelines',
              component: () => import('@/views/index/guidelines.vue'),
              meta: { title: '用户协议' }
            }
          ]
        },
        {
          path: '/post/:id',
          name: 'post',
          component: () => import('@/views/post.vue')
        },
      ],
      redirect: 'index'
    },
    {
      path: '/*',
      name: '404',
      component: () => import('@/views/404.vue')
    }
  ]
});

NProgress.configure({ showSpinner: false });

router.beforeEach((to, from, next) => {
  NProgress.start();
  next();
});

router.afterEach(() => {
  NProgress.done();
});

export default router;
