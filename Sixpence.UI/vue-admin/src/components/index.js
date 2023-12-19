import Vue from 'vue';
import register from './register.vue';
import spHeader from './spHeader.vue';
import spButtonList from './spButtonList.vue';
import spIcon from './spIcon.vue';
import spList from './spList.vue';
import spSection from './spSection.vue';
import spTag from './spTag.vue';
import spEditor from './spEditor.vue';
import spSelect from './spSelect.vue';
import spSwitch from './spSwitch.vue';
import spComments from './spComments.vue';
import spCard from './spCard.vue';
import spLogin from './spLogin.vue';
import cloudUpload from './cloudUploadDialog.vue';

const components = [
  { name: register.name, component: register },
  { name: spHeader.name, component: spHeader },
  { name: spButtonList.name, component: spButtonList },
  { name: spIcon.name, component: spIcon },
  { name: spList.name, component: spList },
  { name: spSection.name, component: spSection },
  { name: spTag.name, component: spTag },
  { name: spEditor.name, component: spEditor },
  { name: spSelect.name, component: spSelect },
  { name: spSwitch.name, component: spSwitch },
  { name: spComments.name, component: spComments },
  { name: spCard.name, component: spCard },
  { name: spLogin.name, component: spLogin },
  { name: cloudUpload.name, component: cloudUpload },
];

const install = _Vue => {
  components.forEach(item => {
    _Vue.component(item.name, item.component);
  });
};

Vue.use(install);
