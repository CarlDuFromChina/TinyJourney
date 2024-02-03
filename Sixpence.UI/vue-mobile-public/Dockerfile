FROM node:20-alpine as node
WORKDIR /app
COPY ./ /app/
RUN npm install \
    && npm run build

FROM nginx:1.15.2-alpine

COPY ./nginx.conf /etc/nginx/nginx.conf
COPY --from=node /app/dist /usr/share/nginx/html
EXPOSE 80 443
ENTRYPOINT ["nginx", "-g", "daemon off;"]