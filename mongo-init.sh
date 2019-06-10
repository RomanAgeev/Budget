#!/bin/sh

mongo ${MONGO_DATABASE} \
    -u ${MONGO_INITDB_ROOT_USERNAME} \
    -p ${MONGO_INITDB_ROOT_PASSWORD} \
    --authenticationDatabase ${MONGO_INITDB_DATABASE} \
    --eval "db.createUser({ user: '${MONGO_USER}', pwd: '${MONGO_PASSWORD}', roles:[{ role:'dbOwner', db: '${MONGO_DATABASE}' }] });"