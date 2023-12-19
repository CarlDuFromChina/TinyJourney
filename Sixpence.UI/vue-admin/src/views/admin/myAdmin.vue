<template>
  <admin ref="admin">
    <!-- 悬浮菜单 -->
    <div class="hover-menu">
      <div class="button-box">
        <a-button size="large" icon="home" shape="circle" class="green-btn" @click="goIndex"></a-button>
      </div>
      <div class="button-box">
        <a-dropdown>
          <a-menu slot="overlay">
            <a-menu-item key="1" @click="$router.push({ name: 'idea' })" v-show="ideaPrivilege.create">新建想法</a-menu-item>
            <a-menu-item key="3" @click="$router.push({ name: 'postEdit' })" v-show="blogPrivilege.create">新建博客</a-menu-item>
          </a-menu>
          <a-button type="primary" size="large" icon="edit" shape="circle"></a-button>
        </a-dropdown>
      </div>
    </div>
    <!-- 悬浮菜单 -->
  </admin>
</template>

<script>
import { admin } from './core';

export default {
  name: 'myAdmin',
  components: { admin },
  data() {
    return {
      blogPrivilege: {},
      ideaPrivilege: {},
    };
  },
  created() {
    sp.get('api/post/privilege').then(resp => this.blogPrivilege = resp);
    sp.get('api/idea/privilege').then(resp => this.ideaPrivilege = resp);
  },
  methods: {
    handleClick(cmd) {
      if (cmd && typeof cmd === 'function') {
        cmd();
      }
    },
    goIndex() {
      window.open(import.meta.env.VITE_APP_INDEX_URL, '_blank');
    },
  }
};
</script>

<style lang="less" scoped>
.hover-menu {
  position: absolute;
  bottom: 50px;
  right: 50px;
  z-index: 10;

  .button-box {
    height: 40px;
    margin-bottom: 8px;
  }
  .green-btn {
    color: #fff;
    background-color: #52c41a;
    border-color: #52c41a;
  }
}
</style>
