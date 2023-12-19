export default [
  {
    path: '/login',
    name: 'login',
    meta: {
      title: 'Login'
    },
    component: () => import('./login.vue')
  },
  {
    path: '/github-oauth/:id?',
    name: 'githubOAuth',
    meta: {
      title: 'Github OAuth'
    },
    component: () => import('./github.vue')
  },
  {
    path: '/gitee-oauth/:id?',
    name: 'giteeOAuth',
    meta: {
      title: 'Gitee OAuth'
    },
    component: () => import('./gitee.vue')
  }
];
