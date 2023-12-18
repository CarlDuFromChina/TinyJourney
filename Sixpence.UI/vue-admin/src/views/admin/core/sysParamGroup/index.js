export default [
  {
    path: '/admin/options',
    name: 'sysParamGroupList',
    component: () => import('./sysParamGroupList'),
    meta: { title: '选项集' }
  }
];
