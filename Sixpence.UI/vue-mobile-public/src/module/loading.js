import Mint from 'mint-ui';

export default {
  data() {
    return {
      loading: false
    };
  },
  methods: {
    async loadData() {
      var Indicator = Mint.Indicator;
      Indicator.open();
      this.loading = true;
      try {
        if (this.fetch && typeof this.fetch === 'function') {
          await this.fetch();
        }
      } catch (error) {
        this.$message.alert(error);
      } finally {
        this.loading = false;
        setTimeout(() => {
          Indicator.close();
        }, 200);
      }
    }
  }
};
