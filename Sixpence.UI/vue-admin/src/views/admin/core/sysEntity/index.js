export default [
  {
    path: '/admin/db',
    name: 'sysEntityList',
    component: () => import('./sysEntityList.vue'),
    meta: { title: '实体管理' }
  }
];
