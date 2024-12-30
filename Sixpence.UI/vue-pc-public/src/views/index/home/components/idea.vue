<template>
  <sp-card title="想法" :loading="loading" :empty="!data || data.length == 0">
    <a-timeline style="padding-top: 8px;">
      <a-timeline-item v-for="(item, index) in data" :key="index">
        <div style="height: 35px;">
          <a-avatar style="width: 30px; height: 30px; margin-right: 4px;" :src="getAvatar(item.created_by)" :alt="item.created_by_name" />
          <span>{{ item.created_at | moment('YYYY-MM-DD HH:mm') }}</span>
        </div>
        <span v-html="item.content"></span>
      </a-timeline-item>
    </a-timeline>
  </sp-card>
</template>

<script>
import { pagination } from '@/mixins';

export default {
  name: 'idea',
  mixins: [pagination],
  data() {
    return {
      data: [],
      controllerName: 'idea',
      pageIndex: 1,
      pageSize: 5,
      loading: false
    };
  },
  created() {
    this.loadData();
  },
  methods: {
    getAvatar(id) {
      return sp.getAvatar(id);
    },
    loadData() {
      if (this.loading) {
        return;
      }
      this.loading = true;
      let url = `api/${this.controllerName}/search?searchList=&pageSize=${this.pageSize}&pageIndex=${this.pageIndex}`;
      sp.get(url)
        .then(resp => {
          if (resp && resp.data) {
            this.data = resp.data;
            this.total = resp.RecordCount;
          } else {
            this.data = resp;
          }
          setTimeout(() => {
            this.loading = false;
          }, 200);
        })
        .catch(error => {
          this.$message.error(error);
        });
    }
  }
};
</script>
