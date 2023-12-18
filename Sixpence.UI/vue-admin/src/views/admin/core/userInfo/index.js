export default [
  {
    path: '/admin/users',
    name: 'userInfoList',
    component: () => import('./userInfoList'),
    meta: { title: '用户信息' }
  }
];
