import { defineConfig, loadEnv } from 'vite';
import vue from '@vitejs/plugin-vue2';
import { createSvgIconsPlugin } from 'vite-plugin-svg-icons';
import { createHtmlPlugin } from 'vite-plugin-html';
import path from 'path';

var isProd = process.env.NODE_ENV === 'production';
const cdn = {
  css: ['https://gcore.jsdelivr.net/npm/ant-design-vue@1.7.8/dist/antd.min.css'],
  js: [
    'https://gcore.jsdelivr.net/npm/moment@2.29.1/moment.min.js',
    'https://gcore.jsdelivr.net/npm/marked@2.1.3/marked.min.js',
    'https://gcore.jsdelivr.net/npm/echarts@5.3.1/dist/echarts.min.js',
    'https://gcore.jsdelivr.net/npm/wangeditor@4.7.8/dist/wangEditor.min.js',
    'https://gcore.jsdelivr.net/npm/vue@2.6.14/dist/vue.min.js',
    'https://gcore.jsdelivr.net/npm/vue-router@3.5.3/dist/vue-router.min.js',
    'https://gcore.jsdelivr.net/npm/vuex@3.6.2/dist/vuex.min.js',
    'https://gcore.jsdelivr.net/npm/ant-design-vue@1.7.8/dist/antd.min.js',
    'https://gcore.jsdelivr.net/npm/mavon-editor@2.10.4/dist/mavon-editor.min.js'
  ]
};

// https://vitejs.dev/config/
export default defineConfig(( mode ) => {
  const env = loadEnv(mode, process.cwd());

  return {
    plugins: [
      vue(),
      createSvgIconsPlugin({
        iconDirs: [path.resolve(process.cwd(), 'src/assets/icons')],
        symbolId: 'sp-blog-[name]'
      }),
      createHtmlPlugin({
        // minify: isProd,
        entry: '/src/main.js',
        template: 'public/index.html',
        inject: {
          data: {
            title: env.VITE_APP_TITLE,
            cdn: isProd ? cdn : {}
          }
        }
      })
    ],
    resolve: {
      alias: {
        '@': path.resolve(__dirname, 'src')
      }
    },
    build: {
      outDir: 'dist',
      rollupOptions: {
        external: isProd
          ? {
              moment: 'moment',
              marked: 'marked',
              echarts: 'echarts',
              wangeditor: 'wangEditor',
              vue: 'Vue',
              'vue-router': 'VueRouter',
              vuex: 'Vuex',
              'ant-design-vue': 'antd',
              'mavon-editor': 'MavonEditor'
            }
          : []
      }
    }
  }
});
