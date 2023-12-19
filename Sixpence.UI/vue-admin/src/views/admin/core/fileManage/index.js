export default [
  {
    path: '/admin/filemanage',
    name: 'fileManage',
    component: () => import('./fileManage.vue'),
    meta: { title: '文件列表' }
  }
];
