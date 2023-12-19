export default [
  {
    path: '/admin/users',
    name: 'userInfoList',
    component: () => import('./userInfoList.vue'),
    meta: { title: '用户信息' }
  }
];
