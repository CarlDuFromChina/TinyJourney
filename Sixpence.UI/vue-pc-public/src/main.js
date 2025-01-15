import App from './App.vue';
import Vue from 'vue';
import Vuex from 'vuex';
import Antd from 'ant-design-vue';
import router from './router';
import moment from 'moment';
import './lib/zh-cn';
import 'ant-design-vue/dist/antd.css';
import './lib';
import './components';
import './style/index.less';
import './directives';
import 'current-device';
import 'virtual:svg-icons-register'

Vue.config.productionTip = false;
Vue.use(Antd);
Vue.use(moment);
Vue.use(Vuex);

Vue.prototype.$bus = new Vue();
Vue.filter('moment', (data, formatStr) => (sp.isNullOrEmpty(data) ? '' : moment(data).format(formatStr)));
moment.locale('zh-cn');
Vue.prototype.$moment = moment;

// 如果是移动端则跳转到移动端应用
if (window.device.mobile()) {
  const currentUrl = new URL(window.location.href);
  const newOrigin = import.meta.env.VITE_APP_MOBILE_URL;
  window.location.href = newOrigin + currentUrl.pathname + currentUrl.search + currentUrl.hash;
} else {
  /* eslint-disable no-new */
  new Vue({
    router,
    render: h => h(App),
  }).$mount('#app');
}

document.title = import.meta.env.VITE_APP_TITLE;
