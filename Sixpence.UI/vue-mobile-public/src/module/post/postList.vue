<template>
  <sp-view>
    <sp-content>
      <mt-search v-model="searchValue" placeholder="输入博客名快速搜索"></mt-search>
      <div v-infinite-scroll="loadData" :infinite-scroll-disabled="loading" infinite-scroll-distance="10" class="list">
        <div v-for="row in list" :key="row.id" class="card item" @click="goReadonly(row.id)">
          <div class="avatar">
            <img :src="getDownloadUrl(row)" alt="" />
          </div>
          <div class="content">
            <div class="title">{{ row.title }}</div>
            <div class="meta">
              <span class="author"><sp-icon name="sp-blog-people" :size="11"></sp-icon> {{ row.created_by_name }}</span>
              <span class="date">{{ row.created_at | moment('YYYY-MM-DD') }}</span>
            </div>
            <div class="info">
              <div class="brief">{{ row.brief }}</div>
            </div>
          </div>
        </div>
      </div>
      <sp-error type="no-content" v-show="list.length === 0"></sp-error>
    </sp-content>
  </sp-view>
</template>

<script>
import loading from '../loading';

export default {
  name: 'index',
  mixins: [loading],
  data() {
    return {
      searchValue: '',
      list: [],
      isLoadedAll: false,
      pageIndex: 1,
      pageSize: 10,
      total: 0
    };
  },
  watch: {
    searchValue(value) {
      this.init();
      this.$nextTick(() => {
        this.loadData();
      });
    }
  },
  methods: {
    init() {
      this.list = [];
      this.isLoadedAll = false;
      this.pageIndex = 1;
      this.pageSize = 10;
      this.total = 0;
    },
    getDownloadUrl(item) {
      return sp.getDownloadUrl(item.surface_url);
    },
    goReadonly(id) {
      this.$router.push({ name: 'post', params: { id: id } });
    },
    fetch() {
      sp.get(
        `${sp.getServerUrl()}api/post/search?searchValue=${this.searchValue}&pageSize=${this.pageSize}&pageIndex=${
          this.pageIndex
        }&searchList=&viewId=463BE7FE-5435-4841-A365-C9C946C0D655`
      ).then(resp => {
        this.total = resp.count;
        this.list = this.list.concat(resp.data);
        this.isLoadedAll = this.pageSize * this.pageIndex >= this.total;
        this.pageIndex++;
      });
    }
  }
};
</script>

<style lang="less" scoped>
/deep/ .mint-search {
  height: auto;
}

.list {
  padding: 8px;
  .item {
    background: #fff;
    padding: 8px;
    margin-top: 8px;
  }
}
.card {
  display: flex;
  .avatar {
    border-radius: 4px;
    img {
      width: 133px;
      height: 90px;
      border-radius: 4px;
    }
  }
  .content {
    padding-left: 16px;
    width: 100%;
    display: flex;
    flex-direction: column;
    text-align: left;
    .title {
      display: inline-block;
      width: 168px;
      color: #222222;
      font-size: 14px;
      height: 1.5em;
      line-height: 1.5em;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      padding-bottom: 6px;
    }
    .brief {
      display: -webkit-box;
      -webkit-box-orient: vertical;
      -webkit-line-clamp: 2;
      overflow: hidden;
      line-height: 1.5em;
      height: 3em;
    }
    .meta {
      display: inline-flex;
      align-items: baseline;
      gap: 8px;
    }
    .author {
      color: #666666;
      font-size: 12px;
      line-height: 1.2em;
    }
    .date {
      color: #666666;
      font-size: 12px;
      line-height: 1.2em;
    }
    .info {
      color: #888888;
      font-size: 12px;
      padding-top: 6px;
    }
  }
}
</style>
