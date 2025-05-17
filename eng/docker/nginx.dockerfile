FROM nginx:1.25.4-alpine3.18
RUN apk update
RUN apk del curl  --purge
RUN apk upgrade
EXPOSE 8080
RUN rm /etc/nginx/conf.d/default.conf
COPY ./nginx/proxy.conf /etc/nginx/conf.d/
COPY ./nginx/reverse-proxy.conf /etc/nginx/conf.d/
COPY ./nginx/nginx.conf /etc/nginx/