export default [
  {
    path: '/admin/role',
    name: 'role',
    component: () => import('./roleList.vue'),
    meta: { title: '角色列表' }
  }
];
