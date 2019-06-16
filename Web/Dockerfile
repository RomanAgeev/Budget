FROM node:9 as client
WORKDIR /app

COPY client/package*.json ./

RUN npm i

COPY client/ ./

RUN npm run parcel:production

FROM node:9-alpine as server
WORKDIR /app

COPY server/package*.json ./

RUN npm i

COPY server/ ./
COPY --from=client /app/dist ./public/

ENV PORT=80

EXPOSE 80
ENTRYPOINT [ "npm", "start" ]
