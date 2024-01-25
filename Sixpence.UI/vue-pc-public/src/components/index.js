import Vue from 'vue';
import spIcon from './spIcon.vue';
import spSection from './spSection.vue';
import spMenu from './spMenu.vue';
import spMenuItem from './spMenuItem.vue';
import spBlogCard from './spBlogCard.vue';
import spCard from './spCard.vue';
import blogMenu from './blogMenu.vue';

const components = [
  { name: spIcon.name, component: spIcon },
  { name: spSection.name, component: spSection },
  { name: spMenu.name, component: spMenu },
  { name: spMenuItem.name, component: spMenuItem },
  { name: spBlogCard.name, component: spBlogCard },
  { name: spCard.name, component: spCard },
  { name: blogMenu.name, component: blogMenu }
];

const install = _Vue => {
  components.forEach(item => {
    _Vue.component(item.name, item.component);
  });
};

Vue.use(install);
