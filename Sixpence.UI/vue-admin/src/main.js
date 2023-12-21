import App from './App.vue';
import Vue from 'vue';
import Vuex from 'vuex';
import Antd from 'ant-design-vue';
import router from './router';
import store from './store';
import moment from 'moment';
import './lib/zh-cn';
import mavonEditor from 'mavon-editor';
import 'mavon-editor/dist/css/index.css';
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
Vue.use(mavonEditor);

Vue.prototype.$bus = new Vue();
Vue.filter('moment', (data, formatStr) => (sp.isNullOrEmpty(data) ? '' : moment(data).format(formatStr)));
moment.locale('zh-cn');
Vue.prototype.$moment = moment;

/* eslint-disable no-new */
new Vue({
  router,
  store,
  render: h => h(App),
}).$mount('#app');