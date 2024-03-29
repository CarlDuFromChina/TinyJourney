import axios from 'axios';
import { trimLast } from './common.js';

function getServerUrl() {
  let url = axios.defaults.baseURL || window.origin;
  return url.charAt(url.length - 1) === '/' ? url : url + '/';
}

function getDownloadUrl(value, isUrl = true) {
  if (sp.isNullOrEmpty(value)) {
    return '';
  }
  const url = isUrl ? value : `/api/sys_file/download?objectId=${value}`;
  if (url.charAt(0) === '/') {
    return `${trimLast(getServerUrl(), '/')}${url}`;
  }
  return `${getServerUrl()}${url}`;
}

function isTrue(val) {
  return val === 'true' || val === true;
}

export default {
  getServerUrl,
  getDownloadUrl,
  isTrue
};
