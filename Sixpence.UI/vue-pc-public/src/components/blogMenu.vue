<template>
  <sp-menu ref="menu" :menus="menus" @menu-change="menuChange" class="blog-menu">
    <template slot="menus">
      <sp-menu-item>
        <a-input-search
          ref="searchInput"
          @focus="searchFocus"
          @blur="searchBlur"
          v-model="searchValue"
          placeholder="输入博客名快速搜索"
          class="search"
          enter-button
          @search="onSearch"
        />
      </sp-menu-item>
    </template>
  </sp-menu>
</template>

<script>
export default {
  name: 'blog-menu',
  data() {
    return {
      basicMenus: [
        {
          name: '主页',
          route: 'home'
        },
        {
          name: '分类',
          route: 'categories'
        },
        {
          name: '项目仓库',
          icon: 'github',
          url: 'https://github.com/CarlDuFromChina/TinyJourney'
        }
      ],
      searchValue: '',
    };
  },
  created() {
    if (this.$route.query.search) {
      this.searchValue = this.$route.query.search;
      this.onSearch(this.searchValue);
    }
  },
  computed: {
    menus: function() {
      if (!sp.isNullOrEmpty(this.$store.getters.website)) {
        const menus = [...this.basicMenus];
        menus.splice(2, 0, { name: '小红书', url: this.$store.getters.website.red_book_url });
        return menus;
      }
      return this.basicMenus;
    }
  },
  methods: {
    // 菜单切换
    menuChange() {
      var indexVM = document.getElementById('index');
      if (indexVM) {
        indexVM.scrollIntoView();
      }
    },
    onSearch(value) {
      this.$router.push({ name: 'home', query: { search: value } }, () => this.$refs.menu.setCurrentMenu());
    },
    searchFocus() {
      this.$refs.searchInput.$el.style.width = '300px';
    },
    searchBlur() {
      this.$refs.searchInput.$el.style.width = '200px';
    }
  }
};
</script>

<style lang="less" scoped>
.search {
  width: 200px;
  vertical-align: middle;
  transition: width 0.3s;
}
</style>
