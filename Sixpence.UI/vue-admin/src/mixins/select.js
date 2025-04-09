export default {
  data() {
    return {
      selectNameList: [], // 选项集，如['category', 'tag']
      selectDataList: {}, // 选项数据集，如{category: [{ name: '', value: ''}]}
    };
  },
  created() {
    this.loadSelectDataList();
  },
  methods: {
    async loadSelectDataList() {
      if (!sp.isNullOrEmpty(this.selectNameList)) {
        for (let i = 0; i < this.selectNameList.length; i++) {
          const entityName = this.selectNameList[i];
          const result = await sp.get(`api/${entityName}/options`);
          this.$set(this.selectDataList, entityName, result);
        }
      }
    }
  }
};
