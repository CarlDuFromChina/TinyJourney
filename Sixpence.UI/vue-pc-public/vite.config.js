import { defineConfig, loadEnv } from 'vite';
import vue from '@vitejs/plugin-vue2';
import { createSvgIconsPlugin } from 'vite-plugin-svg-icons';
import { createHtmlPlugin } from 'vite-plugin-html';
import { Plugin as importCDNPlugin, autoComplete } from 'vite-plugin-cdn-import';
import path from 'path';

var isProd = process.env.NODE_ENV === 'production';

// https://vitejs.dev/config/
export default defineConfig(( mode ) => {
  const env = loadEnv(mode, process.cwd());

  return {
    plugins: [
      vue(),
      importCDNPlugin({
        modules: [
          {
            name: 'vue',
            var: 'Vue',
            path: 'https://lf9-cdn-tos.bytecdntp.com/cdn/expire-1-M/vue/2.6.14/vue.min.js'
          },
          {
            name: 'vue-router',
            var: 'VueRouter',
            path: 'https://lf9-cdn-tos.bytecdntp.com/cdn/expire-1-M/vue-router/3.5.3/vue-router.min.js'
          },
          {
            name: 'vuex',
            var: 'Vuex',
            path: 'https://lf26-cdn-tos.bytecdntp.com/cdn/expire-1-M/vuex/3.6.2/vuex.min.js'
          },
          {
            name: 'moment',
            var: 'moment',
            path: 'https://lf26-cdn-tos.bytecdntp.com/cdn/expire-1-M/moment.js/2.29.1/moment.min.js'
          },
          {
            name: 'ant-design-vue',
            var: 'antd',
            path: 'https://lf3-cdn-tos.bytecdntp.com/cdn/expire-1-M/ant-design-vue/1.7.8/antd.min.js',
            css: 'https://lf6-cdn-tos.bytecdntp.com/cdn/expire-1-M/ant-design-vue/1.7.8/antd.min.css'
          },
          {
            name: 'marked',
            var: 'marked',
            path: 'https://lf9-cdn-tos.bytecdntp.com/cdn/expire-1-M/marked/2.1.3/marked.min.js'
          }
        ]
      }),
      createSvgIconsPlugin({
        iconDirs: [path.resolve(process.cwd(), 'src/assets/icons')],
        symbolId: 'sp-blog-[name]'
      }),
      createHtmlPlugin({
        minify: isProd,
        entry: '/src/main.js',
        template: 'public/index.html',
        inject: {
          data: {
            title: env.VITE_APP_TITLE,
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
    },
    server: {
      port: 8080,
    }
  };
});
