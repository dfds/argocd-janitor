FROM alpine:latest

RUN apk --update add postgresql-client ca-certificates tzdata && rm -rf /var/cache/apk/*

# SSL
RUN wget https://s3.amazonaws.com/rds-downloads/rds-combined-ca-bundle.pem -O /tmp/rds-combined-ca-bundle.pem \
    && mv /tmp/rds-combined-ca-bundle.pem /usr/local/share/ca-certificates/rds-combined-ca-bundle.crt \
    && update-ca-certificates

ENV PGSSLROOTCERT /etc/ssl/certs/ca-certificates.crt
ENV PGSSLMODE verify-full
ENV PGPORT 5432

# timezone
ENV TZ=Europe/Copenhagen
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

WORKDIR /db

COPY . ./

RUN chmod +x init.sh

ENTRYPOINT [ "./init.sh" ]
