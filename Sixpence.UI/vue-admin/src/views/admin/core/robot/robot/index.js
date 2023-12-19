export default [{
  name: 'robot-list',
  path: '/admin/robot',
  component: () => import('./robot.vue'),
  meta: { title: '机器人' }
}];
