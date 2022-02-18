﻿using Bot.Data;
using Bot.Enums;
using Discord;
using Microsoft.Extensions.DependencyInjection;

namespace Bot.Extensions;

public static class EmbedCreator
{
	public static async Task<EmbedBuilder> CreateBasicEmbed(RestAction action, IServiceProvider provider,
		IUser author = null)
	{
		EmbedBuilder embed = new()
		{
			Timestamp = DateTime.Now
		};

		embed.Color = action switch
		{
			RestAction.Updated => Color.Orange,
			RestAction.Deleted => Color.Red,
			RestAction.Created => Color.Green,
			_ => embed.Color
		};

		if (author != null)
			embed.WithAuthor(author);

		var config = await provider.GetRequiredService<SettingsRepository>().GetAppSettings();

		if (!string.IsNullOrEmpty(config.ServiceBaseUrl))
			embed.Url = config.ServiceBaseUrl;

		return embed;
	}
}