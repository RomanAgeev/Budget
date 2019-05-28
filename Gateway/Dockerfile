FROM node:9
WORKDIR /app

COPY package*.json ./
COPY .npmrc ./

RUN npm i

COPY . .

ENV PORT=80

EXPOSE 80
ENTRYPOINT [ "npm", "start" ]