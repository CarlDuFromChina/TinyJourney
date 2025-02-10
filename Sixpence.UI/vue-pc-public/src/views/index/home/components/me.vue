<template>
  <sp-card v-if="showUser" :loading="loading">
    <div class="about">
      <img :src="lifePhoto" alt="" />
      <div class="label-container">
        <label style="color: #2A73CC"><sp-icon name="sp-blog-nipple" style="padding-right: 2px;position: relative; "></sp-icon>{{ name }}</label>
        <label style="color: #FF7043"><sp-icon name="sp-blog-birthday" style="padding-right: 2px;position: relative; top: -1px;"></sp-icon>{{ birthday }}</label>
      </div>
      <p>{{ introduction }}</p>
    </div>

  </sp-card>
</template>

<script>
export default {
  name: 'me',
  data() {
    return {
      showUser: true,
      lifePhoto: import('../../../../assets/images/pic_err.png'),
      name: '',
      introduction: '',
      birthday: '',
      loading: true
    };
  },
  async created() {
    var user = await sp.get('api/index/owner');
    if (user) {
      if (user.life_photo) {
        this.lifePhoto = sp.getDownloadUrl(user.life_photo, false);
      } else {
        const image = await import('../../../../assets/images/pic_err.png');
        this.lifePhoto = image.default;
      }
      this.introduction = user.introduction;
      this.name = user.name;
      if (user.birthday)
        this.birthday = this.$moment(user.birthday).format('YYYY-MM-DD');
    } else {
      this.showUser = false;
    }
    setTimeout(() => {
      this.loading = false;
    }, 200);
  },
  methods: {}
};
</script>

<style lang="less" scoped>
@import url('./card');

.about {
  img {
    width: 100%;
    margin-bottom: 24px;
    border-radius: 5px;
  }
  p, label {
    color: rgba(0, 0, 0, 0.65);
    font-size: 14px;
    line-height: 1.2;
  }
}

.label-container {
  display: flex;
  justify-content: space-between;
  padding: 4px 0px;
}
</style>
