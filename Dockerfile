FROM microsoft/dotnet:2.2.1-aspnetcore-runtime

# SSL
RUN curl -o /tmp/rds-combined-ca-bundle.pem https://s3.amazonaws.com/rds-downloads/rds-combined-ca-bundle.pem \
    && mv /tmp/rds-combined-ca-bundle.pem /usr/local/share/ca-certificates/rds-combined-ca-bundle.crt \
    && update-ca-certificates

WORKDIR /app
COPY ./output/app ./

# OpenSSL cert for Kafka
RUN curl -sS -o /app/cert.pem https://curl.haxx.se/ca/cacert.pem
ENV ARGOJANITOR_KAFKA_SSL_CA_LOCATION=/app/cert.pem

ENTRYPOINT [ "dotnet", "ArgoJanitor.WebApi.dll" ]
