import { EmbedFieldData, MessageEmbed } from "discord.js";
import dayjs from "dayjs";
import { api } from "../../api";
import { Command } from "../../structure/Command";
import { IMenuApiResponse } from "../../types/menu";

export default new Command({
  name: "menu_tomorrow",
  description: "Shows tomorrow's menu",
  run: async ({ interaction, client }) => {
    const res = await api.get<IMenuApiResponse>(
      `/Menu/${dayjs().add(1, "days").format("DD.MM.YYYY")}`
    );

    const mappedFoods: EmbedFieldData[] = res.data.data.foods.map((food) => ({
      name: "Food",
      value: `${food.name} - ${food.calories.toString()} kalori`,
    }));

    const exampleEmbed = new MessageEmbed()
      .setTitle(`Menu ${res.data.data.date}`)
      .setURL("https://yemekhane.cu.edu.tr")
      .setThumbnail(
        client.user?.avatarURL() ||
          "https://www.cu.edu.tr/storage/anamenu_icerikleri/May2018/cukurova_logo.png"
      )
      .addFields(...mappedFoods)
      .setTimestamp();

    await interaction.followUp({
      embeds: [exampleEmbed],
    });
  },
});
