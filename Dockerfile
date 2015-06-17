FROM microsoft/aspnet:1.0.0-beta4

ADD . /app

WORKDIR /app

RUN ["dnu", "restore"]

ENV SERVICE_PORT=5006
ENV CATALOG_SERVICE_URL=http://46.101.191.124:5984

ENTRYPOINT sleep 99999999999 | dnx . Microsoft.AspNet.Hosting --server Kestrel --server.urls http://localhost:$SERVICE_PORT
