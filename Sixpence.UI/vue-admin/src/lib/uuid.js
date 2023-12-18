import UUID from 'uuidjs';
import common from './common';

/**
 * 创建一个 Guid
 */
export function generate() {
  return UUID.generate();
}

/**
 * 判断 uuid 是否一致
 * @param {string} value1
 * @param {string} value2
 */
export function isSame(value1, value2) {
  if (common.isNull() || common.isNull(value2)) {
    return false;
  }

  return (
    value1.replace(/[{}]/g, '').toUpperCase() ===
    value2.replace(/[{}]/g, '').toUpperCase()
  );
}

export default {
  generate,
  isSame,
};
