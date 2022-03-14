import { EmbedFieldData, MessageEmbed } from "discord.js";
import { api } from "../../api";
import { Command } from "../../structure/Command";
import { IMenuApiResponse } from "../../types/menu";

export default new Command({
  name: "menu",
  description: "Shows specific date's menu",
  options: [
    {
      name: "target_date",
      description: "DD.MM.YYYY specific date format",
      required: true,
      type: "STRING",
    },
  ],
  run: async ({ interaction, client, args }) => {
    const specifiedDate = args.data[0].value;

    if (!specifiedDate) {
      return interaction.followUp(
        "Please give valid date. Example Format: 23.03.2022"
      );
    }

    try {
      const res = await api.get<IMenuApiResponse>(`/Menu/${specifiedDate}`);

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

      return interaction.followUp({
        embeds: [exampleEmbed],
      });
    } catch (error) {
      console.log("err :", error.message);
      return interaction.followUp("Menu not found");
    }
  },
});
