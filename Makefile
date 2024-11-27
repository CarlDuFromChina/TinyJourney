build-vue-admin:
	cd ./Sixpence.UI/vue-admin \
	&& docker buildx build --platform=linux/amd64 -t tj-vue-admin . -f Dockerfile \
	&& docker tag tj-vue-admin carldu/tj-vue-admin:1.0.0
	docker push carldu/tj-vue-admin:1.0.0

build-server:
	cd ./Sixpence.TinyJourney \
	docker buildx build --platform=linux/amd64 -t tj-server . -f Dockerfile
	docker tag tj-server carldu/tj-server:1.0.0
	docker push carldu/tj-server:1.0.0

build-vue-pc-public:
	cd ./Sixpence.UI/vue-pc-public \
	&& docker buildx build --platform=linux/amd64 -t tj-vue-pc-public . -f Dockerfile \
	&& docker tag tj-vue-pc-public carldu/tj-vue-pc-public:1.0.0
	docker push carldu/tj-vue-pc-public:1.0.0

build-vue-mobile-public:
	cd ./Sixpence.UI/vue-mobile-public \
	&& docker buildx build --platform=linux/amd64 -t tj-vue-mobile-public . -f Dockerfile \
	&& docker tag tj-vue-mobile-public carldu/tj-vue-mobile-public:1.0.0
	docker push carldu/tj-vue-mobile-public:1.0.0