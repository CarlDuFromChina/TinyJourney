<template>
  <sp-list :controller-name="controllerName" :operations="operations" :columns="columns" :edit-component="editComponent"></sp-list>
</template>

<script>
import sysEntityEdit from './sysEntityEdit.vue';

export default {
  name: 'sysEntityList',
  data() {
    return {
      controllerName: 'sys_entity',
      operations: ['new', 'delete', 'search'],
      columns: [
        { prop: 'name', label: '名称' },
        { prop: 'code', label: '表名' },
        { prop: 'schema', label: '模式' },
        { prop: 'created_at', label: '创建日期', type: 'datetime' },
        { prop: 'updated_at', label: '最后修改日期', type: 'datetime' }
      ],
      editComponent: sysEntityEdit
    };
  }
};
</script>

<style></style>
