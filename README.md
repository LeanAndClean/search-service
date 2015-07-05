##Search Service

##Service configuration

```
export SERVICE_PORT=5006
export DISCOVERY_SERVICE_URLS=http://46.101.138.192:8500,http://46.101.191.124:8500
```

##Deploy configuration

```
export SERVICE_VERSION=0.0.14
export PUBLISH_SERVICE=<ip>:<port>
export DEPLOY_SERVICE=<ip>:<port>
```

##Build

`docker build -t search-service .`

##Run locally

`docker run -t -i -p $SERVICE_PORT:$SERVICE_PORT search-service .`

##Publish into private registry

```
docker tag search-service $PUBLISH_SERVICE/search-service:$SERVICE_VERSION
docker push $PUBLISH_SERVICE/search-service:$SERVICE_VERSION
```

##Deploy

```
curl -X POST \
-H 'Content-Type: application/json' \
-H 'X-Service-Key: pdE4.JVg43HyxCEMWvsFvu6bdFV7LwA7YPii' \
http://$DEPLOY_SERVICE/api/containers?pull=true \
-d '{  
  "name":"'$PUBLISH_SERVICE'/search-service:'$SERVICE_VERSION'",
  "cpus":0.1,
  "memory":64,
  "environment":{
    "SERVICE_CHECK_SCRIPT":"curl -s http://$SERVICE_CONTAINER_IP:$SERVICE_CONTAINER_PORT/healthcheck",
    "SERVICE_PORT":"$SERVICE_PORT",
    "DISCOVERY_SERVICE_URLS":"http://46.101.138.192:8500,http://46.101.191.124:8500",
    "LOG":"true"
  },
  "hostname":"",
  "domain":"",
  "type":"service",
  "network_mode":"bridge",
  "links":{},
  "volumes":[],
  "bind_ports":[  
    {  
       "proto":"tcp",
       "host_ip":null,
       "port":'$SERVICE_PORT',
       "container_port":'$SERVICE_PORT'
    }
  ],
  "labels":[],
  "publish":false,
  "privileged":false,
  "restart_policy":{  
    "name":"no"
  }
}'
```

##API

###Health check

```
curl -X GET \
-H 'Content-Type: application/json' \
http://localhost:$SERVICE_PORT/healthcheck
```

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

###Replicate

**Replicate data

```
curl -X GET \
-H 'Content-Type: application/json' \
http://localhost:$SERVICE_PORT/replicate
```
