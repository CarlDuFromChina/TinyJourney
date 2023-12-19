export default [
  {
    path: '/admin/config',
    name: 'sysConfig',
    component: () => import('./sysConfigList.vue'),
    meta: { title: '系统参数' }
  }
];
