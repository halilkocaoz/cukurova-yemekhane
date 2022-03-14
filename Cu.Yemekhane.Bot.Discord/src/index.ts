import "dotenv/config";
import DiscordClient from "./structure/DiscordClient";

export const client = new DiscordClient();

client.start();
