export default [
  {
    path: '/admin/db',
    name: 'sysEntityList',
    component: () => import('./sysEntityList'),
    meta: { title: '实体管理' }
  }
];
