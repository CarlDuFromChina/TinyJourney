import post from './post';
import error from './error';
import idea from './idea';

export default [{
  path: '/',
  redirect: 'index'
}, {
  name: 'index',
  path: '/index',
  component: () => import('./index.vue'),
  children: [].concat(post, idea),
  redirect: '/index/postList'
}, {
  path: '/post/:id',
  component: () => import('./post/postReadonly.vue')
}].concat(error);
