import Vue from 'vue';
import App from './App.vue';
import router from './router';
import components from './components';
import 'mint-ui/lib/style.css';
import './style/index.less';
import moment from 'moment';
import './lib/extension';
import store from './store';
import 'current-device';
import 'virtual:svg-icons-register';

Vue.use(moment);
Vue.use(components);
Vue.filter('moment', (data, formatStr) => (sp.isNullOrEmpty(data) ? '' : moment(data).format(formatStr)));
moment.locale('zh-cn');
Vue.prototype.$bus = new Vue();
Vue.config.productionTip = false;

if (!window.device.mobile()) {
  window.location.href = import.meta.env.VITE_APP_PC_URL;
} else {
  (async() => {
    var Mint = await import('mint-ui');
    Vue.use(Mint.default);
    Vue.prototype.$message = Mint.MessageBox;
    /* eslint-disable no-new */
    new Vue({
      router,
      store,
      render: h => h(App)
    }).$mount('#app');
  })();
}
