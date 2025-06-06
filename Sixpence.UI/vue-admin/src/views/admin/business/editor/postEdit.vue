<template>
  <div class="blog blog__edit">
    <div class="blog-body">
      <div class="blog-bodywrapper">
        <div class="blog-bodywrapper-markdown">
          <mavon-editor
            v-model="data.content"
            ref="editor"
            :toolbars="toolbar"
            @imgAdd="imgAdd"
            @change="change"
            @save="saveData"
          >
            <!-- 左工具栏前加入自定义按钮 -->
            <template slot="right-toolbar-before">
              <a-popover v-if="showAutoSave">
                <template slot="content">
                  <p>{{ saveStatus?.text }}</p>
                </template>
                <a-icon
                  :type="saveStatus?.icon"
                  :spin="saveStatusValue === 'wait'"
                  :style="{ fontSize: '14px', color: saveStatus?.color, paddingRight: '8px' }"
                  @click="saveDraft">
                </a-icon>
              </a-popover>
              <a-tooltip>
                <template slot="title">AI生成内容</template>
                <a-icon type="code" :style="{ fontSize: '14px'}" @click="() => isAIGenerateVisible = true" />
              </a-tooltip>
            </template>
          </mavon-editor>
        </div>
      </div>
    </div>
    <a-modal title="发布文章" v-model="publishModalVisible" @ok="savePost">
      <a-form-model ref="form" :model="data" :rules="rules">
        <a-row>
          <a-col>
            <a-form-model-item label="标题" prop="title">
              <a-input v-model="data.title"></a-input>
            </a-form-model-item>
          </a-col>
        </a-row>
        <a-row>
          <a-col>
            <a-form-model-item label="分类" prop="post_type">
              <sp-select
                v-model="data.post_type"
                :options="selectDataList.category"
                @change="item => (data.post_type_name = item.name)"
              ></sp-select>
            </a-form-model-item>
          </a-col>
        </a-row>
        <a-row>
          <a-col>
            <a-form-model-item label="摘要">
              <a-spin :spinning="briefLoading">
                <a-input v-model="data.brief" type="textarea" placeholder="请输入摘要，若为空则自动生成" :auto-size="{ minRows: 2, maxRows: 4 }" allow-clear />
                <a-button type="primary" style="float: right;" @click="generate">AI生成</a-button>
              </a-spin>
            </a-form-model-item>
          </a-col>
        </a-row>
        <a-row>
          <a-col :span="8">
            <a-form-model-item label="发布">
              <a-switch v-model="data.is_show"></a-switch>
            </a-form-model-item>
          </a-col>
          <a-col :span="8">
            <a-form-model-item label="禁止评论">
              <a-switch v-model="data.disable_comment"></a-switch>
            </a-form-model-item>
          </a-col>
          <a-col :span="8">
            <a-form-model-item label="置顶">
              <a-switch v-model="data.is_pop"></a-switch>
            </a-form-model-item>
          </a-col>
        </a-row>
        <a-row>
          <a-col>
            <a-form-model-item label="标签">
              <sp-tag :tags="tags" @change="changeTags"></sp-tag>
            </a-form-model-item>
          </a-col>
        </a-row>
        <a-row>
          <a-col>
            <a-form-model-item label="封面">
              <a-spin :spinning="loading">
                <a-upload
                  :action="baseUrl"
                  ref="files"
                  :file-list="fileList"
                  :beforeUpload="beforeUpload"
                  :remove="removeSurface"
                  list-type="picture"
                  :openFileDialogOnClick="false"
                >
                  <a-button type="primary" @click="openCloudUpload" icon="upload">上传</a-button>
                </a-upload>
              </a-spin>
            </a-form-model-item>
          </a-col>
        </a-row>
      </a-form-model>
      <span slot="footer" class="dialog-footer">
        <a-button @click="publishModalVisible = false">取 消</a-button>
        <a-button type="primary" @click="savePost" :loading="loading">确 定</a-button>
      </span>
    </a-modal>
      <a-modal title="AI 自动生成" v-model="isAIGenerateVisible" @ok="generateContent" @cancel="cancelGnenerateContent" :confirmLoading="aiGenerateLoading">
        <a-spin tip="AI自动生成中" :spinning="aiGenerateLoading" :style="{ zIndex: '9999'}">
          <a-form :label-col="{ span: 4 }" :wrapper-col="{ span: 20 }">
            <a-form-item label="提示词">
              <a-input
                v-model="promptInputValue"
                placeholder="输入提示词 AI 自动生成内容"
              ></a-input>
            </a-form-item>
          </a-form>
        </a-spin>
      </a-modal>
    <cloud-upload ref="cloudUpload" @selected="selected"></cloud-upload>
  </div>
</template>

<script>
import { edit, select } from '@/mixins';
import { htmlToText } from 'html-to-text';
import { toolbar } from './mavon';
import axios from 'axios';

export default {
  name: 'post-edit',
  mixins: [edit, select],
  data() {
    return {
      toolbar,
      draft: {},
      isDirty: false,
      seconds: 60,
      configCode: 'enable_draft',
      saveStatusValue: '',
      unwatch: null,
      statusList: [
        {
          value: 'wait',
          icon: 'sync',
          text: '60秒后备份',
          color: '#000000a6'
        },
        {
          value: 'success',
          icon: 'check-circle',
          text: '草稿保存成功',
          color: '#52c41a'
        },
        {
          value: 'fail',
          icon: 'close-circle',
          text: '草稿保存失败',
          color: '#ff4d4f'
        }
      ],
      html: '',
      configs: {},
      publishModalVisible: false,
      controllerName: 'post',
      selectNameList: ['category'],
      fileList: [],
      baseUrl: sp.getServerUrl(),
      tags: [],
      data: {
        disable_comment: false,
        content: ''
      },
      token: this.$store.getters.getToken,
      rules: {
        title: [{ required: true, message: '请输入名称', trigger: 'blur' }],
        post_type: [{ required: true, message: '请选择分类', trigger: 'blur' }],
      },
      briefLoading: false,
      isAIGenerateVisible: false,
      aiGenerateLoading: false,
      promptInputValue: ''
    };
  },
  async created() {
    // 开启监听
    this.$on('open-watch', () => {
      this.unwatch = this.$watch('data.content', (v1, v2) => {
          if (v1 === v2) {
            return;
          }

          // 倒计时保存草稿
          if (!this.isDirty) {
            this.isDirty = true;
            this.saveStatusValue = 'wait';
            if (this.secondId) {
              window.clearInterval(this.secondId);
            }
            this.secondId = setInterval(() => {
              if (this.seconds === 0) {
                this.saveDraft();
              } else {
                this.seconds -= 1;
                this.statusList[0].text = `${this.seconds}秒后备份`;
              }
            }, 1000);
          }
        }
      );
    });

    // 编辑草稿，打开监听
    var draftid = this.$route.params.draftId;
    if (!sp.isNullOrEmpty(draftid)) {
      var resp = await sp.get(`api/draft/${draftid}`);
      this.draft = resp;
      const { content, title } = resp;
      this.data.id = draftid;
      this.data.content = content;
      this.data.title = title;
      this.$emit('open-watch');
      return;
    }

    // 新建博客，打开监听
    if (this.pageState === 'create') {
      this.$emit('open-watch');
    }
  },
  mounted() {
    // 全屏显示
    var editorVM = this.$refs.editor;
    editorVM.s_fullScreen = true;
    editorVM.fullscreen(true, editorVM.d_value);

    // 关闭检查
    window.onbeforeunload = (e) => {
      e = e || window.event;
      if (e) {
        e.returnValue = '关闭提示';
      }
      if (this.unwatch && typeof this.unwatch === 'function') {
        this.unwatch();
      }
      if (this.secondId) {
        window.clearInterval(this.secondId);
      }
      return '关闭提示';
    }
  },
  computed: {
    // 请求头
    headers() {
      return {
        Authorization: 'Bearer ' + this.token
      };
    },
    showAutoSave() {
      return !!this.saveStatusValue;
    },
    saveStatus() {
      return this.statusList.find(item => item.value === this.saveStatusValue);
    }
  },
  watch: {
    'data.post_type': {
      handler(newVal, oldVal) {
        if (this.selectDataList.category) {
          let item = this.selectDataList.category.find(item => item.Value === oldVal);
          if (item) {
            const index = this.tags.indexOf(item.Name);
            if (index !== -1) {
              this.tags.splice(index, 1);
            }
          }

          item = this.selectDataList.category.find(item => item.Value === newVal);
          if (item && new Set(this.tags.concat(item.Name)).size === this.tags.concat(item.Name).length) {
            this.tags.push(item.Name);
          }
        }
      }
    }
  },
  methods: {
    selected(item) {
      this.data.surface_id = item.surface_id;
      this.data.surface_url = item.surface_url;
      this.data.big_surface_id = item.big_surface_id;
      this.data.big_surface_url = item.big_surface_url;
      this.fileList = [
        {
          uid: '0',
          status: 'done',
          name: 'surface',
          url: sp.getDownloadUrl(this.data.surface_url)
        }
      ];
    },
    openCloudUpload() {
      this.$refs.cloudUpload.visible = true;
    },
    async loadComplete() {
      if (!sp.isNullOrEmpty(this.data.surface_id)) {
        this.fileList = [
          {
            uid: '0',
            status: 'done',
            name: 'surface',
            url: sp.getDownloadUrl(this.data.surface_url)
          }
        ];
      }
      if (!sp.isNullOrEmpty(this.data.tags)) {
        this.data.tags = JSON.parse(this.data.tags);
        this.tags = this.data.tags;
      }

      // 查找是否有草稿需要恢复
      if (this.pageState === 'edit') {
        var draft = await sp.get(`api/draft/post/${this.data.id}`);
        if (draft) {
          this.draft = draft;
          this.restoreDraft()
        } else {
          this.$emit('open-watch');
        }
      }
    },
    beforeUpload() {
      return false;
    },
    // 移除封页
    removeSurface() {
      this.data.surface_id = '';
      this.data.surface_url = '';
      this.data.big_surface_id = '';
      this.data.big_surface_url = '';
      this.fileList = [];
      this.$message.success('删除成功！');
    },
    // 将图片上传到服务器，返回地址替换到md中
    imgAdd(pos, file) {
      const url = '/api/sys_file/upload_image?fileType=blog_content&objectId=' + (this.data.id || this.draft.post_id || '');
      const formData = new FormData();
      formData.append('file', file);
      sp.post(url, formData, this.headers).then(resp => {
        let oStr = `(${pos})`;
        let nStr = `(${sp.getDownloadUrl(resp.download_url)})`;
        let index = this.data.content.indexOf(oStr);
        let str = this.data.content.replace(oStr, '');
        let insertStr = (soure, start, newStr) => {
          return soure.slice(0, start) + newStr + soure.slice(start);
        };
        this.data.content = insertStr(str, index, nStr);
      });
    },
    // 所有操作都会被解析重新渲染
    change(value, render) {
      this.html = render; // render 为 markdown 解析后的结果[html]
    },
    saveData() {
      this.publishModalVisible = true;
    },
    // 保存博客
    savePost() {
      this.$refs.form.validate(async valid => {
        if (valid) {
          this.publishModalVisible = false;
          if (sp.isNullOrEmpty(this.data.brief)) {
            this.data.brief = htmlToText(this.html, { baseElement: 'p', limits: { ellipsis: '...', maxInputLength: 200 } });
          }
          if (this.tags) {
            this.data.tags = JSON.stringify(this.tags);
          }
          this.data.html_content = this.html;
          try {
            var id = await sp.post('api/post/save', this.data);
            this.$confirm({
              title: '是否查看?',
              content: '是否查看该博客？',
              okText: '是',
              cancelText: '否',
              onOk: () => {
                window.onbeforeunload = null;
                window.location.href = `${import.meta.env.VITE_APP_INDEX_URL}/post/${id}`;
              },
              onCancel: () => {
                window.onbeforeunload = null;
                window.close();
              }
            });
          } catch (error) {
            this.$message.error(error);
          }
        }
      });
    },
    changeTags(val) {
      this.tags = val;
    },
    /**
     * 获取草稿
     **/
    restoreDraft() {
      this.$confirm({
        title: '是否恢复?',
        content: '发现您尚未保存该博客，是否恢复上次备份内容？',
        okText: '恢复',
        cancelText: '取消',
        onOk: () => {
          const { post_id, content, title } = this.draft;
          this.data.id = post_id;
          this.data.content = content;
          this.data.title = title;
          sp.delete(`/api/draft/${this.draft.id}`)
            .finally(() => this.$emit('open-watch'));
        },
        onCancel: () => {
          sp.delete(`/api/draft/${this.draft.id}`)
            .then(() => this.$message.info('已删除草稿'))
            .finally(() => this.$emit('open-watch'));
        }
      });
    },
    /**
     * 保存草稿
     */
    saveDraft() {
      this.draft.title = this.data.title || '草稿';
      this.draft.content = this.data.content;
      this.draft.images = this.data.images;
      if (!sp.isNullOrEmpty(this.data.id)) {
        this.draft.post_id = this.data.id;
      }
      sp.post('api/draft/save', this.draft)
        .then(resp => {
          this.saveStatusValue = 'success';
          this.isDirty = false;
          this.draft.id = resp;
          this.$message.success('保存成功');
        })
        .catch(() => {
          this.saveStatusValue = 'fail';
          this.$message.error('保存失败')
        })
        .finally(() => {
          this.seconds = 60;
          clearInterval(this.secondId);
        });
    },
    generate() {
      sp.post('api/post/generate_summary', this.data.content)
        .then(resp => {
          this.data.brief = resp;
          this.$message.success('生成成功');
        })
        .catch(() => {
          this.$message.error('生成失败');
        })
        .finally(() => {
          this.briefLoading = false;
        });
    },
    generateContent() {
      if (sp.isNullOrEmpty(this.promptInputValue)) {
        this.$message.error('请输入提示词');
        return;
      }
      this.aiGenerateLoading = true;
      this.cancelToken = axios.CancelToken.source();
      sp.post('api/post/generate_markdown', { prompt: this.promptInputValue }, {
        timeout: 2 * 60 * 1000,
        cancelToken: this.cancelToken.token
      })
        .then(resp => {
          this.data.content = resp;
          this.$message.success('生成成功');
        })
        .catch((error) => {
          if (axios.isCancel(error) || error === '取消AI生成') {
            this.$message.error('取消AI生成');
          } else {
            this.$message.error('生成失败');
          }
        })
        .finally(() => {
          this.aiGenerateLoading = false;
          this.isAIGenerateVisible = false;
        });
    },
    cancelGnenerateContent() {
      if (this.cancelToken) {
        this.cancelToken.cancel('取消AI生成');
      }
    }
  }
};
</script>

<style lang="less" scoped>
.blog {
  height: 100%;
  &.blog__edit {
    .blog-body {
      color: #212529;
      height: calc(100% - 60px);
      .blog-bodywrapper {
        height: 100%;
        .blog-bodywrapper-markdown {
          height: 100%;
          .v-note-wrapper {
            height: 100%;
          }
        }
      }
    }
  }
}

/deep/ .v-note-wrapper.fullscreen {
  z-index: 100;
}
</style>
