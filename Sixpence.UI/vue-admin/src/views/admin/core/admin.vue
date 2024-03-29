<template>
  <a-layout class="layout-home">
    <!--侧边栏左-->
    <a-layout-sider v-model="collapsed" :trigger="null" :width="240" collapsible>
      <!--LOGO-->
      <div class="logo" @click="$router.push({ name: 'workplace' })">
        <sp-icon name="sp-blog-baby" :size="32"></sp-icon>
        <span v-if="!collapsed">{{ appTitle }}</span>
      </div>
      <!--菜单栏-->
      <div class="sider">
        <a-menu theme="dark" mode="inline" :open-keys="openKeys" @openChange="onOpenChange" :inlineIndent="24">
          <a-sub-menu v-for="(item, index) in menus" :key="index">
            <span slot="title">
              <a-icon :type="item.icon" /><span>{{ item.title }}</span>
            </span>
            <a-menu-item v-for="item2 in item.children" :key="`/admin/${item2.router}`" @click="handleClick">
              {{ item2.title }}
            </a-menu-item>
          </a-sub-menu>
        </a-menu>
      </div>
    </a-layout-sider>
    <!--侧边栏右-->
    <a-layout>
      <!--导航栏-->
      <a-layout-header :style="{ background: '#fff', padding: '0 20px 0 0', textAlign: 'right' }">
        <!--左侧信息版-->
        <a-icon
          class="trigger"
          :type="collapsed ? 'menu-unfold' : 'menu-fold'"
          :style="{float:'left'}"
          @click="() => (collapsed = !collapsed)"
        />
        <!--自定义按钮-->
        <slot></slot>
        <!--用户头像-->
        <a-dropdown>
          <a-menu slot="overlay">
            <a-menu-item key="1" @click="() => this.$router.push({ name: 'notification' })"
              ><a-badge :dot="messageCount > 0"><a-icon type="notification" />消息通知</a-badge></a-menu-item
            >
            <a-menu-item key="2" @click="() => (userInfoEditVisible = true)"><a-icon type="setting" />我的设置</a-menu-item>
            <a-menu-item key="3" @click="logout"><a-icon type="logout" />退出登录</a-menu-item>
          </a-menu>
          <a-badge :count="messageCount">
            <a-avatar :src="imageUrl" shape="circle" style="cursor: pointer" />
          </a-badge>
        </a-dropdown>
      </a-layout-header>
      <!--内容区域-->
      <a-layout-content :style="{ margin: '24px 16px', overflow: 'hidden' }">
        <div :style="{ background: '#fff', height: '100%' }">
          <router-view :key="$route.path"></router-view>
        </div>
      </a-layout-content>
    </a-layout>
    <!--用户模态窗-->
    <a-modal v-model="userInfoEditVisible" title="我的设置" @ok="saveUserInfo" width="60%" okText="确认" cancelText="取消">
      <user-info-edit ref="userInfoEdit" :related-attr="userParam" @close="userInfoEditVisible = false"></user-info-edit>
    </a-modal>
  </a-layout>
</template>

<script>
import { clearAuth } from '@/lib/login';
import userInfoEdit from './userInfo/userInfoEdit.vue';

export default {
  components: { userInfoEdit },
  data() {
    return {
      appTitle: import.meta.env.VITE_APP_TITLE,
      collapsed: false,
      menus: [],
      defaultOpenedsArray: [],
      openKeys: [],
      data: {},
      rules: {
        password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
        password2: [{ required: true, message: '请再次输入密码', trigger: 'blur' }]
      },
      imageUrl: '',
      userParam: {
        id: sp.getUserId()
      },
      userInfoEditVisible: false,
      messageCount: 0
    };
  },
  created() {
    this.getMenu();
    this.imageUrl = `${sp.getServerUrl()}api/system/avatar/${sp.getUserId()}`;
    sp.get(`api/sys_user/${sp.getUserId()}`).then(resp => {
      this.$store.commit('updateUser', resp);
    });
    sp.get('api/message_remind/unread_message_count').then(resp => {
      this.messageCount = resp.total;
    });
  },
  methods: {
    goHome() {
      this.$router.push({
        name: 'home'
      });
    },
    getMenu() {
      const searchList = [
        {
          Name: 'is_enable',
          Value: true
        }
      ];
      sp.get(`api/sys_menu/search?searchList=${JSON.stringify(searchList)}`)
        .then(resp => {
          resp.data.forEach(e => {
            const menu = {
              title: e.name,
              router: e.router,
              children: [],
              icon: e.icon
            };
            if (e.children && e.children.length > 0) {
              menu.children = e.children.map(item => ({
                title: item.name,
                router: item.router,
                icon: item.icon
              }));
            }
            this.menus.push(menu);
          });
        })
        .catch(resp => {
          this.$message.error(resp);
        });
    },
    handleClick({ keyPath }) {
      if (sp.isNullOrEmpty(keyPath[0])) {
        this.$message.error('发生错误，请检查菜单地址是否正确！');
        return;
      }
      if (keyPath[0] !== this.$route.path) {
        this.$router.push({ path: keyPath[0] });
      }
    },
    logout() {
      this.$message.success('退出成功');
      clearAuth(this.$store);
      this.$router.push({ name: 'login' });
    },
    onOpenChange(openKeys) {
      const latestOpenKey = openKeys.find(key => this.openKeys.indexOf(key) === -1);
      const rootSubmenuKeys = this.menus.map((item, index) => index);
      if (rootSubmenuKeys.indexOf(latestOpenKey) === -1) {
        this.openKeys = openKeys;
      } else {
        this.openKeys = !sp.isNil(latestOpenKey) ? [latestOpenKey] : [];
      }
    },
    saveUserInfo() {
      this.$refs.userInfoEdit.saveData();
    }
  }
};
</script>

<style lang="less">
body,
html {
  margin: 0px;
  width: 100%;
  height: 100%;
}
</style>
<style lang="less" scoped>
.trigger {
  font-size: 18px;
  line-height: 64px;
  padding: 0 24px;
  cursor: pointer;
  transition: color 0.3s;
}
.logo {
  height: 32px;
  margin: 12px 24px;
  overflow: hidden;
  cursor: pointer;
  > span {
    color: #fff;
    font-size: 20px;
    font-weight: 600;
    margin-left: 12px;
  }
}
.layout-home {
  height: 100%;
  overflow: hidden;
}

.sider {
  overflow: hidden auto;
  height: 100%;
  &::-webkit-scrollbar {
    width: 6px;
    height: 6px;
  }

  &::-webkit-scrollbar-track {
    background: hsla(0, 0%, 100%, 0.15);
    border-radius: 3px;
    -webkit-box-shadow: inset 0 0 5px rgba(37, 37, 37, 0.05);
  }

  &::-webkit-scrollbar-thumb {
    background: hsla(0, 0%, 100%, 0.2);
    border-radius: 3px;
    -webkit-box-shadow: inset 0 0 5px hsla(0, 0%, 100%, 0.05);
  }
}
</style>
