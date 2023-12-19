export default [
  {
    path: '/admin/gallery',
    name: 'gallery',
    component: () => import('./gallery.vue'),
    meta: { title: '图库' }
  }
];
