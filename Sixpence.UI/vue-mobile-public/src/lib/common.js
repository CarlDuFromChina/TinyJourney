import store from '../store';
import axios from 'axios';

export function isNil(value) {
  return value === undefined || value === null;
}

export function isNull(value) {
  return value === undefined || value === null;
}

export function isNumber(value) {
  return typeof value === 'number' || Object.prototype.toString.call(value) === '[Object Number]';
}

export function isObject(value) {
  const type = typeof value;
  return value != null && (type === 'object' || type === 'function');
}

export function isFunction(value) {
  return typeof value === 'function';
}

export function isNullOrEmpty(value) {
  if (isNull(value)) {
    return true;
  }
  switch (Object.prototype.toString.call(value)) {
    case '[object String]':
      return value.trim().length === 0;
    case '[object Array]':
      return value.length === 0;
    case '[object Object]':
      return Object.keys(value).length === 0;
    default:
      throw new TypeError();
  }
}

function getServerUrl() {
  let url = axios.defaults.baseURL || window.origin;
  return url.charAt(url.length - 1) === '/' ? url : url + '/';
}

export function getDownloadUrl(value, isUrl = true) {
  if (sp.isNullOrEmpty(value)) {
    return '';
  }
  const url = isUrl ? value : `/api/sys_file/download?objectId=${value}`;
  if (url.charAt(0) === '/') {
    return `${trimLast(getServerUrl(), '/')}${url}`;
  }
  return `${getServerUrl()}${url}`;
}

export function getUserId() {
  return store.getters.getUserId;
}

export function isTrue(val) {
  return val === 'true' || val === true;
}

function trimLast(str, char) {
  if (str.charAt(str.length - 1) === char) {
    return str.substring(0, str.length - 1);
  }
  return str;
}

export default {
  isNil,
  isNull,
  isNumber,
  isObject,
  isFunction,
  isNullOrEmpty,
  getServerUrl,
  getDownloadUrl,
  getUserId,
  isTrue
};
