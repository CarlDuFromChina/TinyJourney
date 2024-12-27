<template>
  <sp-card title="想法" :loading="loading" :empty="!data || data.length == 0">
    <a-timeline>
      <a-timeline-item v-for="(item, index) in data" :key="index">
        <span>{{ item.created_at | moment('YYYY-MM-DD HH:mm') }}</span>
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
