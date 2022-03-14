import {
  ApplicationCommandDataResolvable,
  Client,
  ClientEvents,
  Collection,
  Intents,
} from "discord.js";
import glob from "glob";
import { promisify } from "util";
import { RegisterCommandsOptions } from "../types/client";
import { CommandType } from "../types/Command";
import { Event } from "./Event";

const globPromise = promisify(glob);

class DiscordClient extends Client {
  commands: Collection<string, any> = new Collection();

  constructor() {
    if (!process.env.DISCORD_BOT_TOKEN) {
      throw new Error("DISCORD_BOT_TOKEN must be defined");
    }
    super({ intents: [Intents.FLAGS.GUILDS] });
  }

  start() {
    this.login(process.env.DISCORD_BOT_TOKEN);
    this.registerModules();
  }

  async importFile(filePath: string) {
    return (await import(filePath))?.default;
  }

  async registerCommands({ commands, guildId }: RegisterCommandsOptions) {
    if (guildId) {
      this.guilds.cache.get(guildId)?.commands.set(commands);
      console.log(`Registering commands to ${guildId}`);
    } else {
      this.application?.commands.set(commands);
      console.log("Registering global commands");
    }
  }

  async registerModules() {
    const slashCommands: ApplicationCommandDataResolvable[] = [];
    const commandFiles = await globPromise(
      `${__dirname}/../commands/*/*{.ts,.js}`
    );
    commandFiles.forEach(async (filePath) => {
      const command: CommandType = await this.importFile(filePath);
      if (!command.name) return;
      console.log(command);

      this.commands.set(command.name, command);
      slashCommands.push(command);
    });

    this.on("ready", () => {
      console.log("Bot is ready");
    });

    this.on("guildCreate", (guild) => {
      this.registerCommands({
        commands: slashCommands,
        guildId: guild.id,
      });
    });

    // Event
    const eventFiles = await globPromise(`${__dirname}/../events/*{.ts,.js}`);
    eventFiles.forEach(async (filePath) => {
      const event: Event<keyof ClientEvents> = await this.importFile(filePath);
      this.on(event.event, event.run);
    });
  }
}

export default DiscordClient;
