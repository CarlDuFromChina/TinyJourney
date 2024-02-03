import Vue from 'vue';
import App from './App.vue';
import router from './router';
import components from './components';
import Mint from 'mint-ui';
import 'mint-ui/lib/style.css';
import './style/index.less';
import moment from 'moment';
import './lib/extension';
import store from './store';
import 'current-device';
import './assets/icons';

Vue.use(moment);
Vue.use(Mint);
Vue.use(components);
Vue.filter('moment', (data, formatStr) => (sp.isNullOrEmpty(data) ? '' : moment(data).format(formatStr)));
moment.locale('zh-cn');
Vue.prototype.$bus = new Vue();
Vue.config.productionTip = false;
Vue.prototype.$message = Mint.MessageBox;

document.title = process.env.VUE_APP_TITLE;
if (!window.device.mobile()) {
  window.location.href = process.env.VUE_APP_PC_URL;
} else {
  /* eslint-disable no-new */
  new Vue({
    router,
    store,
    render: h => h(App)
  }).$mount('#app');
}
