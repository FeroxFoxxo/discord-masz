using Bot.Abstractions;
using Bot.Extensions;
using Bot.Translators;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Utilities.Translators;

namespace Utilities.Commands;

public class Avatar : Command<Avatar>
{
	[SlashCommand("avatar", "Get the high resolution avatar of a user.")]
	public async Task AvatarCommand([Summary("user", "User to get the avatar from")] IUser user)
	{
		await Context.Interaction.DeferAsync(ephemeral: !GuildConfig.StaffChannels.Contains(Context.Channel.Id));
		await UserAvatar(user.Id.ToString(), false);
	}

	[ComponentInteraction("avatar-user:*,*")]
	public async Task UserAvatar(string userId, bool isGuild)
	{
		IUser user = Context.Client.GetUser(ulong.Parse(userId));
		IGuildUser gUser = Context.Guild.GetUser(ulong.Parse(userId));
		var guildAvail = false;

		if (gUser is { GuildAvatarId: { } })
			guildAvail = true;

		if (isGuild && !guildAvail)
			isGuild = false;

		if (Context.Interaction is SocketMessageComponent castInteraction)
		{
			var avatarUrl = isGuild ? gUser.GetGuildAvatarUrl(size: 1024) : user.GetAvatarOrDefaultUrl(size: 1024);
			var translator = Translator.Get<UtilityTranslator>();

			var embed = new EmbedBuilder()
				.WithTitle(isGuild ? translator.GuildAvatarUrl() : translator.AvatarUrl())
				.WithFooter($"{Translator.Get<BotTranslator>().UserId()}: {(gUser ?? user).Id}")
				.WithUrl(avatarUrl)
				.WithImageUrl(avatarUrl)
				.WithAuthor(gUser)
				.WithColor(Color.Magenta)
				.WithCurrentTimestamp();

			var buttons = new ComponentBuilder();

			if (guildAvail)
				buttons.WithButton(isGuild ? translator.AvatarUrl() : translator.GuildAvatarUrl(), $"avatar-user:{user.Id},{!isGuild}");

			await castInteraction.UpdateAsync(message =>
			{
				message.Embed = embed.Build();
				message.Components = buttons.Build();
			});
		}
	}
}