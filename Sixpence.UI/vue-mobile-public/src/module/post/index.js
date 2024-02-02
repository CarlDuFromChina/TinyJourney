export default [
  {
    name: 'post-list',
    path: '/index/postList',
    component: () => import('./postList.vue')
  },
  {
    name: 'post',
    path: '/index/post/:id',
    component: () => import('./postReadonly.vue')
  }
];
