import JSEncrypt from 'jsencrypt';
import md5 from 'js-md5';
import { encode, decode } from 'js-base64';
import CryptoJS from 'crypto-js';

export default {
  md5: {
    encrypt: (str) => md5(str),
  },
  rsa: {
    encrypt: (str, publickKey) => {
      const jsencrypt = new JSEncrypt({});
      jsencrypt.setPublicKey(publickKey);
      return jsencrypt.encrypt(str);
    },
    decrypt: (str, privateKey) => {
      const jsdecrypt = new JSEncrypt({});
      jsdecrypt.setPrivateKey(privateKey);
      return jsdecrypt.decrypt(str);
    },
  },
  base64: {
    encode: (str) => encode(str),
    decode: (str) => decode(str),
  },
  aes: {
    encrypt: (word, key = '') => {
      return CryptoJS.AES.encrypt(word, key).toString();
    },
    decrypt: (word, key = '') => {
      const bytes = CryptoJS.AES.decrypt(word, key);
      const originTxt = bytes.toString(CryptoJS.enc.Utf8);
      return originTxt;
    },
  },
};
