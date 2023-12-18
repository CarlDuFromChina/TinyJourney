import axios from 'axios';
import { isNullOrEmpty } from './common';

axios.defaults.timeout = 20000;
axios.defaults.withCredentials = true;

function _handleSuccess(res) {
  if (!res) return;

  if (!res.data) {
    return res.data;
  }

  if (res.data.ErrorCode === 0) {
    return res.data.Data || res.data;
  } else if (!isNullOrEmpty(res.data.ErrorCode) && !isNullOrEmpty(res.data.Message)) {
    return Promise.reject(new Error(res.data.Message));
  } else {
    return res.data;
  }
}

function getErrorMessage(error, defaultMessage = 'Oops！') {
  if (typeof error === 'string' && !isNullOrEmpty(error)) {
    return error;
  }

  if (error && error.Message) {
    return error.Message;
  }

  if (error && error.message) {
    return error.message;
  }

  return defaultMessage;
}

function _handleError(error) {
  const { status, data = {} } = error.response || {};
  let errorMessage;
  if (status === 403) {
    errorMessage = getErrorMessage(data, '您没有权限访问该资源');
  } else if (status === 401) {
    errorMessage = '请重新登录';
  } else if (status === 500) {
    errorMessage = getErrorMessage(data, '系统错误，请联系管理员');
  } else if (status === 404) {
    errorMessage = getErrorMessage(data.Message, '未找到资源');
  } else if (data.Message) {
    errorMessage = data.Message;
  } else if (error.message) {
    errorMessage = error.message;
  } else {
    errorMessage = '服务器开小差了';
  }
  return errorMessage;
}

export default {
  axios,
  get: (url, config) => {
    return new Promise((resolve, reject) => {
      axios
        .get(url, config)
        .then(res => resolve(_handleSuccess(res)))
        .catch(err => reject(_handleError(err)));
    });
  },
  post: (url, data, config) => {
    return new Promise((resolve, reject) => {
      axios
        .post(url, data, config)
        .then(res => resolve(_handleSuccess(res)))
        .catch(err => reject(_handleError(err)));
    });
  },
  delete: (url, config) => {
    return new Promise((resolve, reject) => {
      axios
        .delete(url, config)
        .then(res => resolve(_handleSuccess(res)))
        .catch(err => reject(_handleError(err)));
    });
  },
  put: (url, data, config) => {
    return new Promise((resolve, reject) => {
      axios
        .put(url, data, config)
        .then(res => resolve(_handleSuccess(res)))
        .catch(err => reject(_handleError(err)));
    });
  }
};
