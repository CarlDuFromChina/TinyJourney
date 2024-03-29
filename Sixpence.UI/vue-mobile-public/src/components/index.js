import layout from './layout';
import error from './error.vue';
import icon from './icon.vue';

const prefix = 'sp';
const components = [
  { name: icon.name, component: icon },
  { name: error.name, component: error }
].concat(layout);

const install = _Vue => {
  components.forEach(item => {
    _Vue.component(`${prefix}-${item.name}`, item.component);
  });
};

export default install;
