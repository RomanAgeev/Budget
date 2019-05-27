FROM node:9
WORKDIR /src

COPY package*.json .

RUN npm ci --only=production

COPY . .

EXPOSE 8080
ENTRYPOINT [ "npm", "start" ]