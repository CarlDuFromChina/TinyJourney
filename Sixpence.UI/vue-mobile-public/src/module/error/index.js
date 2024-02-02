export default [
  {
    path: '*',
    redirect: '/404'
  },
  {
    name: '404',
    path: '/404',
    component: () => import('./404.vue')
  },
  {
    name: '401',
    path: '/401',
    component: () => import('./401.vue')
  }
];
