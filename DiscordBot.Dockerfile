FROM node:16.13.0

WORKDIR /app

COPY ./Cu.Yemekhane.Bot.Discord/package.json ./Cu.Yemekhane.Bot.Discord/yarn.lock ./

RUN yarn --pure-lockfile

COPY ./Cu.Yemekhane.Bot.Discord/ .

RUN yarn build

CMD ["yarn", "start"]