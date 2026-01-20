<template>
  <sp-view>
    <sp-header :title="data.title"></sp-header>
    <sp-content>
      <article v-html="content" class="content"></article>
    </sp-content>
  </sp-view>
</template>

<script>
import { marked } from 'marked';

export default {
  name: 'post',
  data() {
    return {
      data: {},
      content: '',
      Id: this.$route.params.id || ''
    };
  },
  created() {
    this.loadData();
  },
  methods: {
    loadData() {
      this.$indicator.open('加载中...');
      sp.get(`api/post/${this.Id}`)
        .then(resp => {
          this.data = resp;
          this.content = marked(resp.content);
        })
        .finally(() => {
          setTimeout(() => {
            this.$indicator.close();
          }, 500);
        });
    }
  }
};
</script>

<style lang="less" scoped>
.content {
  text-align: left;
  padding: 12px;
  color: #2a2a2a;
  line-height: 1.7;
  font-size: 14px;
  word-break: break-word;
  background: #ffffff;
}

.content /deep/ h1,
.content /deep/ h2,
.content /deep/ h3,
.content /deep/ h4,
.content /deep/ h5,
.content /deep/ h6 {
  margin: 16px 0 10px;
  line-height: 1.35;
  font-weight: 600;
  color: #1f1f1f;
}
.content /deep/ h1 {
  font-size: 22px;
}
.content /deep/ h2 {
  font-size: 18px;
}
.content /deep/ h3 {
  font-size: 16px;
}
.content /deep/ h4 {
  font-size: 15px;
}
.content /deep/ h5 {
  font-size: 14px;
}
.content /deep/ h6 {
  font-size: 13px;
  color: #666666;
}

.content /deep/ p {
  margin: 10px 0;
}

.content /deep/ a {
  color: #3b82f6;
  text-decoration: none;
  word-break: break-word;
}
.content /deep/ a:active,
.content /deep/ a:hover {
  text-decoration: underline;
}

.content /deep/ img {
  max-width: 100%;
  height: auto;
  border-radius: 8px;
  display: block;
  margin: 10px auto;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.06);
}

.content /deep/ ul,
.content /deep/ ol {
  margin: 8px 0 8px 20px;
  padding: 0;
}
.content /deep/ li {
  margin: 6px 0;
}

.content /deep/ blockquote {
  margin: 12px 0;
  padding: 10px 12px;
  border-left: 4px solid #e5e7eb;
  background: #f9fafb;
  color: #555555;
  border-radius: 6px;
}

.content /deep/ code {
  font-family: ui-monospace, SFMono-Regular, Menlo, Consolas, "Liberation Mono", monospace;
  font-size: 12px;
  background: #f3f4f6;
  padding: 2px 6px;
  border-radius: 4px;
}

.content /deep/ pre {
  background: #0f172a;
  color: #e2e8f0;
  padding: 12px;
  border-radius: 8px;
  overflow-x: auto;
  font-size: 12px;
  line-height: 1.6;
}
.content /deep/ pre code {
  background: transparent;
  padding: 0;
  color: inherit;
}

.content /deep/ hr {
  border: none;
  border-top: 1px solid #e5e7eb;
  margin: 16px 0;
}

.content /deep/ table {
  width: 100%;
  border-collapse: collapse;
  margin: 10px 0;
  font-size: 13px;
}
.content /deep/ th,
.content /deep/ td {
  border: 1px solid #e5e7eb;
  padding: 8px;
  text-align: left;
}
.content /deep/ th {
  background: #f3f4f6;
  font-weight: 600;
}
</style>
