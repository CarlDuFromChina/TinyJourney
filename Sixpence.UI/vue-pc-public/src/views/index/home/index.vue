<template>
  <a-layout>
    <!-- 博客 -->
    <a-layout-sider width="70%" theme="light">
      <blog-list ref="blogList"></blog-list>
    </a-layout-sider>
    <!-- 博客 -->
    <a-layout-sider width="30%" style="overflow:hidden" theme="light">
      <!-- 关于我 -->
      <me style="margin-bottom:20px;"></me>
      <!-- 关于我 -->

      <!-- 想法 -->
      <idea :pageSize="5" style="margin-bottom:20px;"></idea>
      <!-- 想法 -->

      <div class="more" v-if="website">
        <p>站长：{{ website?.author }}</p>
        <p>邮箱：<a :href="`mailto:${website?.email}`">{{ website?.email }}</a></p>
        <p>版本号：{{ config.version }}</p>
        <p><a href="https://beian.miit.gov.cn">{{ website?.record_no }}</a></p>
      </div>
    </a-layout-sider>
  </a-layout>
</template>

<script>
import { Idea, BlogList, Me } from './components'
import packageConfig from '../../../../package.json';
import { mapState } from 'vuex';

export default {
  name: 'home',
  components: { Idea, BlogList, Me },
  data() {
    return {
      loading: 'false',
      blog: {
        pageSize: 20,
        pageIndex: 1,
        totalRecords: 0,
        allowLoad: () => {
          return this.pageSize * this.pageIndex < this.totalRecords;
        }
      },
      config: packageConfig,
      searchValue: ''
    };
  },
  computed: {
    ...mapState(['website'])
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.searchValue = to.query.search;
      vm.$refs.blogList.onSearch(vm.searchValue);
    });
  },
  beforeRouteUpdate (to, from, next) {
    this.searchValue = to.query.search;
    this.$refs.blogList.onSearch(this.searchValue);
    next();
  },
  methods: {
    loadMore() {
      if (this.blog.allowLoad()) {
        this.blog.pageSize += 10;
        this.$refs.blog.loadData();
      }
    }
  }
};
</script>

<style lang="less" scoped>
/deep/ .ant-layout-sider-light {
  background: #edeef2;
}

.more {
  border-radius: 2px;
  margin-bottom: 1.3rem;
  line-height: 1;
  font-size: 14px;
  color: #9aa3ab;
}
</style>
