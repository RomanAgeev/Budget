FROM node:9-alpine
WORKDIR /app

COPY package*.json ./
COPY .npmrc ./

RUN npm i

COPY . .

RUN npm test

ENV PORT=80

EXPOSE 80
ENTRYPOINT [ "npm", "start" ]