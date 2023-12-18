export default [
  {
    path: '/admin/config',
    name: 'sysConfig',
    component: () => import('./sysConfigList'),
    meta: { title: '系统参数' }
  }
];
