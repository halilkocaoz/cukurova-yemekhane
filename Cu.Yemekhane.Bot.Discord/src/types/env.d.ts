declare namespace NodeJS {
  export interface ProcessEnv extends NodeJS.ProcessEnv {
    NODE_ENV?: "development" | "production";
    DISCORD_BOT_TOKEN: string;
  }
}
