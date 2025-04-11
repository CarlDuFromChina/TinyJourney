<template>
  <vue-waterfall-easy ref="waterfall" @click="showModal" :imgsArr="dataList" @scrollReachBottom="loadData">
    <div slot="waterfall-head">
      <sp-header>
        <div v-if="buttons && buttons.length > 0" style="display:inline-block">
          <sp-button-list :buttons="buttons" @search-change="loadData"></sp-button-list>
        </div>
      </sp-header>
      <a-modal title="图片" v-model="readVisible" v-if="readVisible">
        <img class="big-image" :src="imgUrl" />
        <template slot="footer">
          <a-button type="primary" @click="downloadImg">点击下载</a-button>
          <a-button type="danger" @click="deleteData">删除</a-button>
        </template>
      </a-modal>
      <gallery-edit v-model="editVisible" @saved="refresh"></gallery-edit>
    </div>
  </vue-waterfall-easy>
</template>

<script>
import vueWaterfallEasy from '@sixpence/vue-waterfall-easy';
import galleryEdit from './galleryEdit.vue';
import { pagination } from '@/mixins';

export default {
  name: 'gallery',
  components: { vueWaterfallEasy, galleryEdit },
  mixins: [pagination],
  data() {
    return {
      imgUrl: '',
      readVisible: false,
      editVisible: false,
      isFirstLoad: true,
      busy: false,
      dataList: [],
      loading: false,
      controllerName: 'gallery',
      baseUrl: sp.getServerUrl(),
      buttons: [{ name: 'new', icon: 'plus', operate: () => (this.editVisible = true) }],
      selectData: null,
    };
  },
  computed: {
    customApi() {
      return `api/${this.controllerName}/search?pageIndex=$pageIndex&pagesize=$pageSize&searchValue=&searchList=&viewId=`;
    }
  },
  created() {
    this.loadData();
  },
  methods: {
    refresh() {
      this.pageIndex = 1;
      this.total = 0;
      this.isFirstLoad = true;
      this.dataList = [];
      this.loadData();
    },
    showModal(event, { index, value }) {
      this.imgUrl = value.infoUrl;
      this.readVisible = true;
      this.selectData = this.dataList[index];
    },
    downloadImg() {
      window.open(this.imgUrl, '_blank');
    },
    loadData() {
      if (this.loading) {
        return;
      }
      this.loading = true;

      if (this.pageSize * this.pageIndex >= this.total && !this.isFirstLoad) {
        this.$refs.waterfall.waterfallOver();
      }

      this.busy = true;
      if (!this.isFirstLoad) {
        this.pageIndex += 1;
      }
      sp.get(this.customApi.replace('$pageSize', this.pageSize).replace('$pageIndex', this.pageIndex))
        .then(resp => {
          this.dataList = this.dataList.concat(
            resp.data.map(item => {
              return {
                id: item.id,
                src: sp.getDownloadUrl(item.preview_url),
                name: item.name,
                infoUrl: sp.getDownloadUrl(item.image_url)
              };
            })
          );
          this.total = resp.count;
          this.isFirstLoad = false;
          this.busy = false;
        })
        .finally(() => {
          this.loading = false;
        });
    },
    deleteData(id) {
      this.$confirm({
        title: '是否删除',
        content: '此操作将永久删除选择项, 是否继续?',
        okText: '确认',
        cancelText: '取消',
        onOk: () => {
          sp.delete(`api/${this.controllerName}/${this.selectData.id}`)
            .then(() => {
              this.$message.success('删除成功');
              setTimeout(() => {
                location.reload();
              }, 1000);
            })
            .catch(error => {
              this.$message.error(error);
            });
        },
        onCancel: () => {
          this.$message.info('已取消删除');
        }
      });
    }
  }
};
</script>

<style lang="less" scoped>
/deep/ .vue-waterfall-easy-scroll {
  max-width: 100% !important;
}

/deep/ .vue-waterfall-easy {
  max-width: 100% !important;
  width: calc(100% - 60px) !important;
}

.big-image {
  width: 100%;
  height: 100%;
  max-width: 800px;
  max-height: 600px;
}
</style>
