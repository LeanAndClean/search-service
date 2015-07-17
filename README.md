#Search Service

##Configuration

```
export SERVICE_PORT=5006
export DISCOVERY_SERVICE_URLS=http://46.101.138.192:8500,http://46.101.191.124:8500
export PUBLISH_SERVICE=<ip>:<port>
export SERVICE_VERSION=0.0.14
```

##Build

`docker build -t search-service .`

##Run locally

`docker run -t -i -p $SERVICE_PORT:$SERVICE_PORT search-service .`

##Publish into private registry

```
docker tag search-service:latest $PUBLISH_SERVICE/search-service:$SERVICE_VERSION
docker push $PUBLISH_SERVICE/search-service:$SERVICE_VERSION
```

##API

###Search Items

```
curl -X GET \
-H 'Content-Type: application/json' \
http://localhost:$SERVICE_PORT
```

**Search with filter**

```
curl -X GET \
-H 'Content-Type: application/json' \
--data-urlencode 'where=Name="Peter" or Name="Max"' \
http://localhost:$SERVICE_PORT
```

**Search with pagination**

```
curl -X GET \
-H 'Content-Type: application/json' \
--data-urlencode 'number=2&page=0' \
http://localhost:$SERVICE_PORT
```

###Replicate data

```
curl -X GET \
-H 'Content-Type: application/json' \
http://localhost:$SERVICE_PORT/replicate
```

###Health check
```
curl -X GET \
-H 'Content-Type: application/json' \
http://localhost:$SERVICE_PORT/healthcheck
```
