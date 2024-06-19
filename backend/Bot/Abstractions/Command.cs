using Bot.Data;
using Bot.Exceptions;
using Bot.Models;
using Bot.Services;
using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Messaging.Extensions;
using Microsoft.Extensions.Logging;

namespace Bot.Abstractions;

public abstract class Command<T> : InteractionModuleBase<SocketInteractionContext>
{
    public GuildConfig GuildConfig;
    public Identity Identity;
    public ILogger<T> Logger { get; set; }
    public Translation Translator { get; set; }
    public IdentityManager IdentityManager { get; set; }
    public GuildConfigRepository GuildConfigRepository { get; set; }

    public override async Task BeforeExecuteAsync(ICommandInfo command)
    {
        Logger.LogInformation(
            "{Username} used {CommandName} in {ChannelName} | {GuildName} ({GuildId})",
            Context.User.Username, command.Name, Context.Channel.Name, Context.Guild.Name, Context.Guild.Id
        );

        GuildConfig = await GuildConfigRepository.GetGuildConfig(Context.Guild.Id);

        if (GuildConfig != null)
            Translator.SetLanguage(GuildConfig);
        else
            throw new UnregisteredGuildException(Context.Guild.Id);

        Identity = await IdentityManager.GetIdentity(Context.User);

        if (Identity == null)
            throw new InvalidIdentityException($"Failed to register command identity for '{Context.User.Id}'.");

        try
        {
            await BeforeCommandExecute();
        }
        catch (InteractionException) { }
    }

    public virtual async Task BeforeCommandExecute() => await DeferAsync();

    public async Task<RestInteractionMessage> RespondInteraction(string content = default,
        EmbedBuilder embedBuilder = null, ComponentBuilder componentBuilder = null)
    {
        if (content is not default(string) and not null)
            if (!string.IsNullOrEmpty(content))
                content = content.SanitizeMentions();

        var embed = embedBuilder?.Build();
        var components = componentBuilder?.Build();

        void Properties(MessageProperties msg)
        {
            msg.Content = content;
            msg.Embed = embed;
            msg.Components = components ?? new ComponentBuilder().Build();
        }

        if (Context.Interaction is SocketMessageComponent castInteraction)
            await castInteraction.UpdateAsync(Properties);
        else
            try
            {
                if (Context.Interaction.HasResponded)
                    return await Context.Interaction.ModifyOriginalResponseAsync(Properties);
                await Context.Interaction.RespondAsync(content, embed: embed, components: components);
            }
            catch
            {
                await Context.Interaction.FollowupAsync(content, embed: embed, components: components);
            }

        return null;
    }
}
