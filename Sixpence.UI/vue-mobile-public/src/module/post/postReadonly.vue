<template>
  <sp-view>
    <sp-header :title="data.title"></sp-header>
    <sp-content>
      <article v-html="content" class="content markdown-body"></article>
    </sp-content>
  </sp-view>
</template>

<script>
import marked from 'marked';

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
          console.log(this.content);
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
  padding: 8px;
}

h2 {
  font-size: 16px;
}

/deep/ .markdown-body img {
  max-width: 100%;
  height: auto;
}

/deep/ .markdown-body {
  line-height: 1.5;
  font-size: 14px;
  color: #333;
  word-wrap: break-word;
}

/deep/ .markdown-body h1,
/deep/ .markdown-body h2,
/deep/ .markdown-body h3,
/deep/ .markdown-body h4,
/deep/ .markdown-body h5,
/deep/ .markdown-body h6 {
  margin-top: 1em;
  margin-bottom: 0.5em;
  font-weight: bold;
  border-bottom: 1px solid #eaecef;
}

/deep/ .markdown-body a {
  color: #0366d6;
  text-decoration: none;
}

/deep/.markdown-body code {
  background-color: #f6f8fa;
  padding: 2px 4px;
  border-radius: 3px;
}

/deep/ .markdown-body table {
  border-collapse: collapse;
  width: 100%;
  margin-bottom: 1em;
}

/deep/ .markdown-body th,
/deep/ .markdown-body td {
  border: 1px solid #ddd;
  padding: 0.5em;
}

/deep/ .markdown-body th {
  background-color: #f6f8fa;
}

/deep/ .markdown-body img {
  max-width: 100%;
  height: auto;
}

/deep/ .markdown-body iframe {
  width: 100%;
  height: 30vh;
}
</style>
