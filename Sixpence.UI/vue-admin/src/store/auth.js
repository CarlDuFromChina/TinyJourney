export default {
  state: {
    isLogin: false,
    user_id: '',
    user_name: '',
    token: {} // { RefreshToken, AccessToken }
  },
  getters: {
    isLoggedIn(state) {
      return !!state.isLogin;
    },
    getToken(state) {
      return (state.token.access_token || {}).token_content;
    },
    getUserId(state) {
      return state.user_id;
    },
    getUserName(state) {
      return state.user_name;
    },
    isAccessTokenExpired: (state) => () => {
      const { access_token } = state.token;
      const t1 = new Date(access_token.expires);
      return t1 < new Date();
    },
    isRefreshTokenExpired: (state) => () => {
      const { refresh_token } = state.token;
      const t1 = new Date(refresh_token.expires);
      return t1 < new Date();
    },
    isEmptyToken: (state) => () => {
      return sp.isNullOrEmpty(state.token);
    }
  },
  mutations: {
    changeLogin(state, data) {
      state.isLogin = data;
    },
    updateAuth(state, data) {
      state.token = data.token;
      state.user_id = data.user_id;
      state.user_name = data.user_name;
    },
    changeTokenWithRefreshToken(state) {
      state.token.access_token = state.token.refresh_token;
    },
    clearAuth(state) {
      state.token = {};
      state.user_id = '';
    },
    updateAccessToken(state, data) {
      state.token.access_token = data;
    }
  }
};
