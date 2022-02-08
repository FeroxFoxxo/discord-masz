using Bot.Abstractions;
using Bot.Attributes;
using Bot.Data;
using Bot.Enums;
using Bot.Services;
using Bot.Translators;
using Discord;
using Discord.Interactions;
using Discord.Net;
using Messaging.Translators;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Messaging.Commands;

public class Say : Command<Say>
{
	public GuildConfigRepository GuildConfigRepository { get; set; }

	[Require(RequireCheck.GuildModerator)]
	[SlashCommand("say", "Let the bot send a message.")]
	public async Task SayCommand(
		[Summary("message", "message content the bot shall write")]
		string message,
		[Summary("channel", "channel to write the message in, defaults to current")]
		ITextChannel channel = null)
	{
		if (channel is null)
			if (Context.Channel is ITextChannel txtChannel)
			{
				channel = txtChannel;
			}
			else
			{
				await Context.Interaction.RespondAsync(Translator.Get<BotTranslator>().OnlyTextChannel(),
					ephemeral: true);
				return;
			}

		try
		{
			var createdMessage = await channel.SendMessageAsync(message);

			await Context.Interaction.RespondAsync(Translator.Get<MessagingTranslator>().MessageSent(), ephemeral: true);

			try
			{
				var guildConfig = await GuildConfigRepository.GetGuildConfig(Context.Guild.Id);

				if (!string.IsNullOrEmpty(guildConfig.ModNotificationWebhook))
				{
					await DiscordRest.ExecuteWebhook(
						guildConfig.ModNotificationWebhook,
						null,
						Translator.Get<MessagingTranslator>().SaySent(
							Context.User,
							createdMessage,
							channel
						),
						AllowedMentions.None
					);
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, $"Something went wrong while sending the internal notification for the say command by {Context.User.Id} in {Context.Guild.Id}/{Context.Channel.Id}.");
			}
		}
		catch (HttpException e)
		{
			if (e.HttpCode == HttpStatusCode.Unauthorized)
				await Context.Interaction.RespondAsync(Translator.Get<BotTranslator>().CannotViewOrDeleteInChannel(),
					ephemeral: true);
		}
		catch (Exception e)
		{
			Logger.LogError(e, $"Error while writing message in channel {channel.Id}");
			await Context.Interaction.RespondAsync(Translator.Get<MessagingTranslator>().FailedToSend(), ephemeral: true);
		}
	}
}