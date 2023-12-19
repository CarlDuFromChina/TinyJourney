export default [
  {
    path: '/admin/notification',
    name: 'notification',
    component: () => import('./notification.vue'),
    meta: { title: '消息通知' }
  }
];
