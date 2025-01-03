<template>
  <sp-card v-if="showUser" :loading="loading">
    <div class="about">
      <img :src="lifePhoto" alt="" />
      <div class="label-container">
        <label><sp-icon name="sp-blog-nipple" style="padding-right: 2px;position: relative; "></sp-icon>{{ name }}</label>
        <label><sp-icon name="sp-blog-birthday" style="padding-right: 2px;position: relative; top: -1px;"></sp-icon>{{ birthday }}</label>
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
    var user = await sp.get('api/post/index_user');
    if (user) {
      if (user.life_photo) {
        this.lifePhoto = sp.getDownloadUrl(user.life_photo, false);
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
    color: #bfbfbf;
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
