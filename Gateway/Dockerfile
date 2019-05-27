FROM node:9
WORKDIR /app

COPY package*.json ./
COPY .npmrc ./

RUN npm i

COPY . .

EXPOSE 8080
ENTRYPOINT [ "npm", "start" ]