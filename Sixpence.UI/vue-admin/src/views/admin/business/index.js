import editorRouter from './editor';

export default [
  {
    path: '/admin/workplace',
    name: 'workplace',
    component: () => import('./workplace.vue')
  },
  {
    path: '/admin/post/:category?',
    name: 'post-list',
    component: () => import('./postList.vue'),
    meta: { title: '文章管理' }
  },
  {
    path: '/admin/series',
    name: 'seriesList',
    component: () => import('./seriesList.vue'),
    meta: { title: '博客系列' }
  },
  {
    path: '/admin/category',
    name: 'category',
    component: () => import('./category/categoryList.vue'),
    meta: { title: '文章分类' }
  },
  {
    path: '/admin/idea',
    name: 'idea',
    component: () => import('./idea/ideaList.vue'),
    meta: { title: '想法' }
  },
  {
    path: '/admin/link',
    name: 'link',
    component: () => import('./link/linkList.vue'),
    meta: { title: '链接' }
  },
  {
    path: '/admin/drafts',
    name: 'draft',
    component: () => import('./draftList.vue'),
    meta: { title: '草稿管理' }
  }
].concat(editorRouter);
