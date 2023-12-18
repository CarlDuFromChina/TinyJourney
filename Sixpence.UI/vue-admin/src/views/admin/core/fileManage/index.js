export default [
  {
    path: '/admin/filemanage',
    name: 'fileManage',
    component: () => import('./fileManage'),
    meta: { title: '文件列表' }
  }
];
