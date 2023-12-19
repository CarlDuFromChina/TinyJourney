export default [
  {
    path: '/admin/menus',
    name: 'sysMenuList',
    component: () => import('./sysMenuList.vue'),
    meta: { title: '菜单管理' }
  }
];
