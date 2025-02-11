import sysEntity from './sysEntity';
import sysMenu from './sysMenu';
import userInfo from './userInfo';
import job from './job';
import gallery from './gallery';
import fileManage from './fileManage';
import role from './role';
import admin from './admin.vue';
import notification from './notification';

export { admin };
export default [].concat(sysEntity, sysMenu, userInfo, job, gallery, fileManage, role, notification);
