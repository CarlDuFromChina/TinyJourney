<template>
  <a-modal title="上传" v-model="editVisible" width="850px" destroyOnClose>
    <a-form-model ref="form" :model="data">
      <a-row>
        <a-col>
          <a-form-model-item label="大图">
            <a-spin :spinning="loading">
              <a-upload
                :action="baseUrl"
                ref="files"
                :file-list="bigImage"
                :beforeUpload="beforeUpload"
                list-type="picture"
                key="big-image-upload"
              >
                <a-button type="primary"> <a-icon type="upload" /> 上传</a-button>
              </a-upload>
            </a-spin>
          </a-form-model-item>
        </a-col>
      </a-row>
      <a-row>
        <a-col>
          <vue-cropper
            ref="cropper"
            v-show="!!imgSrc"
            :src="imgSrc"
            :aspect-ratio="16 / 9"
            :viewMode="1"
            :autoCropArea="1"
          ></vue-cropper>
        </a-col>
      </a-row>
      <a-row>
        <a-col>
          <a-form-model-item label="标签">
            <sp-tag :tags="tags" @change="changeTags"></sp-tag>
          </a-form-model-item>
        </a-col>
      </a-row>
    </a-form-model>
    <template slot="footer">
      <a-button type="primary" @click="cropAndUploadImage">保存</a-button>
    </template>
  </a-modal>
</template>

<script>
import { edit } from '@/mixins';
import VueCropper from 'vue-cropperjs';
import 'cropperjs/dist/cropper.css'

export default {
  name: 'galleryEdit',
  mixins: [edit],
  components: { VueCropper },
  model: {
    prop: 'visible',
    event: 'input'
  },
  props: {
    visible: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      controllerName: 'gallery',
      baseUrl: sp.getServerUrl(),
      smallImage: [],
      bigImage: [],
      token: this.$store.getters.getToken,
      tags: [],
      imgSrc: '',
      file: null
    };
  },
  computed: {
    editVisible: {
      get() {
        return this.visible;
      },
      set(value) {
        this.$emit('input', value);
      }
    },
    // 请求头
    headers() {
      return {
        Authorization: 'Bearer ' + this.token
      };
    }
  },
  methods: {
    preSave() {
      if (!this.smallImage || this.smallImage.length === 0) {
        this.$message.error('图片不能为空');
        return false;
      }
      if (!this.bigImage || this.bigImage.length === 0) {
        this.$message.error('图片不能为空');
        return false;
      }
      if (this.tags) {
        this.data.tags = this.tags.join(', ');
      }
      return true;
    },
    postSave() {
      this.$emit('input', false);
      this.$emit('saved');
    },
    changeTags(val) {
      this.tags = val;
    },
    upload(param) {
      const url = '/api/sys_file/upload_big_image?fileType=gallery&objectId=';
      const formData = new FormData();
      formData.append('file', param.file);
      return sp.post(url, formData, this.headers).then(resp => resp);
    },
    // 上传大图
    uploadBigImg(param) {
      return this.upload(param).then(resp => {
        const image = resp[0];
        const thumbnail = resp[1];

        this.data.image_id = image.id;
        this.data.image_url = image.download_url;
        this.bigImage = [
          {
            uid: '0',
            status: 'done',
            name: 'big_image',
            url: sp.getDownloadUrl(image.download_url)
          }
        ];

        this.data.preview_id = thumbnail.id;
        this.data.preview_url = thumbnail.download_url;
        this.smallImage = [
          {
            uid: '0',
            status: 'done',
            name: 'small_image',
            url: sp.getDownloadUrl(thumbnail.download_url)
          }
        ];
      });
    },
    beforeUpload(file, fileList) {
      this.file = file;
      if (fileList && fileList.length > 1) {
        fileList = fileList.slice(-1);
      }
      const reader = new FileReader();
      reader.onload = (e) => {
        this.imgSrc = e.target.result;
        this.$refs.cropper.replace(e.target.result);
      };
      reader.readAsDataURL(file);
      // 阻止自动上传
      return false;
    },
    cropAndUploadImage() {
      var file = this.file;
      var fileName = file.name.replace(/\.jpg$/, '.png');
      this.$refs.cropper.getCroppedCanvas().toBlob(async (blob) => {
        this.file = new File([blob], fileName, { type: blob.type, lastModified: Date.now() });
        await this.uploadBigImg({ file: this.file });
        await this.saveData();
      });
    },
    removeSmallImg() {
      this.data.preview_id = '';
      this.data.preview_url = '';
      this.smallImage = [];
    },
    removeBigImg() {
      this.data.image_id = '';
      this.data.image_url = '';
      this.bigImage = [];
    }
  }
};
</script>

<style></style>
