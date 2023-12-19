export default [
  {
    path: '/admin/job',
    name: 'job',
    component: () => import('./job.vue'),
    meta: { title: '作业管理' }
  }
];
