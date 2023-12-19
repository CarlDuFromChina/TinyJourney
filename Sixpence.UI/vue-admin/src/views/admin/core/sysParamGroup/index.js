export default [
  {
    path: '/admin/options',
    name: 'sysParamGroupList',
    component: () => import('./sysParamGroupList.vue'),
    meta: { title: '选项集' }
  }
];
