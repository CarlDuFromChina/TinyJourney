<template>
  <sp-list ref="list" :controllerName="controllerName" :operations="operations" :columns="columns" :editComponent="editComponent"></sp-list>
</template>

<script>
import editComponent from './categoryEdit.vue';

export default {
  name: 'category-list',
  data() {
    return {
      controllerName: 'category',
      editComponent,
      columns: [
        { prop: 'name', label: '名称' },
        { prop: 'code', label: '编码' },
        { prop: 'index', label: '索引' },
      ],
      operations: ['new', 'delete', 'search']
    };
  }
};
</script>

<style></style>
