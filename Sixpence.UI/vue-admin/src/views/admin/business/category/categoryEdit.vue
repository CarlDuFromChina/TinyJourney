<template>
  <a-form-model ref="form" :model="data">
    <a-form-model-item label="名称">
      <a-input v-model="data.name"></a-input>
    </a-form-model-item>
    <a-form-model-item label="编码">
      <a-input v-model="data.code" :disabled="pageState === 'edit'"></a-input>
    </a-form-model-item>
    <a-form-model-item label="索引">
      <a-input-number v-model="data.index"></a-input-number>
    </a-form-model-item>
  </a-form-model>
</template>

<script>
import { edit } from '@/mixins';

export default {
  name: 'category-edit',
  mixins: [edit],
  data() {
    return {
      controllerName: 'category',
      data: { }
    };
  },
  methods: {
    postSave() {
      setTimeout(() => {
        window.location.reload();
      }, 1000);
    }
  }
};
</script>

<style></style>
