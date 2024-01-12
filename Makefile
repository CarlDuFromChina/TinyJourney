build-vue-admin:
	cd ./Sixpence.UI/vue-admin \
	&& docker buildx build --platform=linux/amd64 -t tj-vue-admin . -f Dockerfile \
	&& docker tag tj-vue-admin carldu/tj-vue-admin:1.0.0

build-server:
	docker buildx build --platform=linux/amd64 -t tj-server . -f Dockerfile
	docker tag tj-server carldu/tj-server:1.0.0