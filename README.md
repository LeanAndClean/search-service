##Search Service

##Service configuration

```
export SERVICE_PORT=5006
export DISCOVERY_SERVICE_URLS=http://46.101.138.192:8500;http://46.101.191.124:8500
```

##Build

`docker build -t search-service .`

##Run locally

`docker run -t -i -p 5006:5006 search-service .`

##Publish into private registry

```
docker tag search-service 46.101.191.124:5000/search-service:0.0.11
docker push 46.101.191.124:5000/search-service:0.0.11
```

##Deploy

###OSX/Linux
```
curl -X POST \
-H 'Content-Type: application/json' \
-H 'X-Service-Key: pdE4.JVg43HyxCEMWvsFvu6bdFV7LwA7YPii' \
http://46.101.191.124:8080/api/containers?pull=true \
-d '{  
  "name":"46.101.191.124:5000/search-service:0.0.11",
  "cpus":0.1,
  "memory":128,
  "environment":{
    "SERVICE_CHECK_SCRIPT":"curl -s http://46.101.191.124:5006/healthcheck",
    "SERVICE_PORT":"5006",
    "DISCOVERY_SERVICE_URLS":"http://46.101.138.192:8500;http://46.101.191.124:8500"
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
       "port":5006,
       "container_port":5006
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

###Windows
```
$Uri = "http://46.101.191.124:8080/api/containers?pull=true"

$Headers = @{
  "X-Service-Key" = "pdE4.JVg43HyxCEMWvsFvu6bdFV7LwA7YPii"
  "Content-Type" = "application/json"
}

$Body = @"
{  
  "name":"46.101.191.124:5000/search-service:0.0.11",
  "cpus":0.1,
  "memory":128,
  "environment":{
    "SERVICE_CHECK_SCRIPT":"curl -s http://46.101.191.124:5006/healthcheck",
    "SERVICE_PORT":"5006",
    "DISCOVERY_SERVICE_URLS":"http://46.101.138.192:8500;http://46.101.191.124:8500"
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
       "port":5006,
       "container_port":5006
    }
  ],
  "labels":[],
  "publish":false,
  "privileged":false,
  "restart_policy":{  
    "name":"no"
  }
}
"@

Invoke-RestMethod -Uri $Uri -Method Post -Headers $Headers -Body $Body
```

##API

###Health check

```
curl -X GET \
-H 'Content-Type: application/json' \
http://localhost:5006/healthcheck
```

###Search Items

```
curl -X GET \
-H 'Content-Type: application/json' \
http://localhost:5006
```

**Search with filter**

```
curl -X GET \
-H 'Content-Type: application/json' \
--data-urlencode 'where=Name="Peter" or Name="Max"' \
http://localhost:5006
```

**Search with pagination**

```
curl -X GET \
-H 'Content-Type: application/json' \
--data-urlencode 'number=2&page=0' \
http://localhost:5006
```

###Replicate

**Replicate data

```
curl -X GET \
-H 'Content-Type: application/json' \
http://localhost:5006/replicate
```
