<template>
  <div ref="header" class="sp-menu-header">
    <div class="sp-menu-header-wrapper">
      <slot name="logo">
        <sp-menu-item class="sp-menu-logo" @click="$router.push({ name: 'index' })">
          <sp-icon name="sp-blog-baby" :size="38"></sp-icon>
          <span class="sp-menu-logo-title">{{ APP_TITLE }}</span>
          <sp-icon class="sp-menu-logo-star" name="sp-blog-star"></sp-icon>
          <span class="sp-menu-logo-subtitle">Powered by ChatGPT</span>
        </sp-menu-item>
      </slot>
      <ul class="sp-menu-list">
        <sp-menu-item v-for="(item, index) in menus" :key="index" @click="menuChange(item, index)" :style="{ color: getColor(index) }">
          <a-icon :type="item.icon" v-if="item.icon"></a-icon>
          {{ item.name }}
        </sp-menu-item>
        <slot name="menus"></slot>
      </ul>
    </div>
    <slot></slot>
  </div>
</template>

<script>

export default {
  name: 'spMenu',
  props: {
    menus: {
      type: Array,
      default: () => []
    }
  },
  data() {
    return {
      APP_TITLE: import.meta.env.VITE_APP_TITLE,
      currentIndex: -1
    };
  },
  mounted() {
    this.setCurrentMenu();
    this.$bus.$on('scroll', val => {
      const ref = this.$refs.header;
      if (val !== 0) {
        ref.style.position = 'fixed';
        ref.style.width = '100%';
        ref.style.zIndex = 99;
        ref.style.top = 0;
      } else {
        ref.style.position = 'relative';
      }
    });
  },
  destroyed() {
    this.$bus.$off('scroll');
  },
  methods: {
    setCurrentMenu() {
      this.menus.forEach((item, index) => {
        if (item.route === this.$route.name) {
          this.currentIndex = index;
        }
      });
    },
    getColor(index) {
      if (this.currentIndex === index) {
        return '#1e80ff';
      }
      return '#222226';
    },
    menuChange(item, index) {
      this.currentIndex = index;
      this.$emit('menu-change');
      if (item.route) {
        this.$router.push({ name: item.route });
      }
      if (item.url) {
        this.$confirm({
          title: '确认',
          content: '即将离开本站，是否继续？',
          okText: '确认',
          cancelText: '取消',
          onOk: () => {
            window.open(item.url, '_blank');
          }
        });
      }
    }
  }
};
</script>

<style lang="less" scoped>
.sp-menu-header {
  color: #222226;
  background: #fff;
  font-size: 14px;
  font-weight: 400;
  height: 50px;
  .sp-menu-logo {
    padding-left: calc((100% - 1200px) / 2);
    cursor: pointer;
    float: left;
    > .svg-icon {
      height: 50px;
    }

    .sp-menu-logo-title {
      color: rgba(0, 0, 0, 0.85);
      padding-left: 8px;
      font-size: 20px;
      font-weight: 600;
    }

    .sp-menu-logo-star {
      padding: 0 4px 0 8px;
    }

    .sp-menu-logo-subtitle {
      font-family: 'Inter', sans-serif;
      font-size: 12px;
      color: #7E8A97;
      font-weight: 500;
      letter-spacing: 0.3px;
    }
  }
  .sp-menu-header-wrapper {
    z-index: 100;
    .sp-menu-list {
      padding-right: calc((100% - 1200px) / 2);
      background: transparent;
      border-bottom: none !important;
      float: right;
      margin: 0;
      border-right: none;
      list-style: none;
      position: relative;
    }
  }
}
</style>
