﻿using Bot.Data;
using Bot.Enums;
using Bot.Models;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bot.Extensions;

public static class EmbedCreator
{
	public static EmbedBuilder CreateColoredEmbed(Color color, Type type)
	{
		var ns = type.Namespace.Split('.');

		return new EmbedBuilder()
			.WithCurrentTimestamp()
			.WithColor(color)
			.WithFooter($"{ns.First()} {ns.Last()}");
	}

	public static async Task<EmbedBuilder> CreateActionEmbed(RestAction action, IServiceProvider provider,
		IUser author = null) => CreateActionEmbed(action, await provider.GetRequiredService<SettingsRepository>().GetAppSettings(), author);

	public static EmbedBuilder CreateActionEmbed(RestAction action, AppSettings settings,
		IUser author = null)
	{
		var embed = new EmbedBuilder()
			.WithCurrentTimestamp()
			.WithColor(action switch
			{
				RestAction.Updated => Color.Orange,
				RestAction.Deleted => Color.Red,
				RestAction.Created => Color.Green,
				_ => Color.Blue
			});

		if (author != null)
			embed.WithAuthor(author);

		var url = settings.GetServiceUrl();

		if (!string.IsNullOrEmpty(url))
			embed.Url = url;

		return embed;
	}
}
