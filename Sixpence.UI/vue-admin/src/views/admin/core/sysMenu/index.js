export default [
  {
    path: '/admin/menus',
    name: 'sysMenuList',
    component: () => import('./sysMenuList'),
    meta: { title: '菜单管理' }
  }
];
