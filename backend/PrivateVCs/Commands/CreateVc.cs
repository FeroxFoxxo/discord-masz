﻿using Bot.Abstractions;
using Bot.Attributes;
using Discord;
using Discord.Interactions;
using PrivateVcs.Data;
using PrivateVcs.Models;
using PrivateVcs.Services;
using System.Linq;
using System.Text.RegularExpressions;

namespace PrivateVcs.Commands;

public class CreateVc : Command<CreateVc>
{
    public PrivateVcConfigRepository ConfigRepository { get; set; }
    public PermissionChecker PermissionChecker { get; set; }

    [SlashCommand("createvc", "Creates a personal voice channel")]
    [BotChannel]
    public async Task CreateVcCommand([Summary("The name of the voice channel")] string vcName)
    {
        var config = await ConfigRepository.SelectPrivateVcConfig(Context.Guild.Id);

        if (config == null)
        {
            await RespondInteraction(
                embedBuilder: new EmbedBuilder()
                    .WithTitle("Private VCs have not been set up in this server!")
                    .WithDescription("Please contact a server administrator to set this up in the config options.")
                    .WithColor(Color.Red)
            );
            return;
        }

        if (vcName.Length is > 100 or 0)
        {
            await RespondInteraction(
                embedBuilder: new EmbedBuilder()
                    .WithTitle("Invalid Channel Name")
                    .WithDescription("Your private channel name must be between 1-100 characters long. " +
                                     $"Your current channel name is {vcName.Length} characters long.")
                    .WithColor(Color.Red)
            );
            return;
        }

        if (Regex.IsMatch(vcName, config.ChannelFilterRegex))
        {
            await RespondInteraction(
                embedBuilder: new EmbedBuilder()
                    .WithTitle("Invalid Channel Name")
                    .WithDescription("Your private channel name cannot go against the channel name filter. " +
                                     "Please inquire about this with the server moderators.")
                    .WithColor(Color.Red)
            );
            return;
        }

        if (Context.Guild.Channels.FirstOrDefault(channel => channel.Name == vcName) != null)
        {
            await RespondInteraction(
                embedBuilder: new EmbedBuilder()
                .WithTitle("Channel Name Already Exists!")
                .WithDescription("Please ensure your channel name does not equal the name of another channel, " +
                                 "as this leads to confusion for other members.")
                .WithColor(Color.Red)
            );
            return;
        }

        IGuildUser user = Context.Guild.GetUser(Context.User.Id);

        if (user.VoiceChannel == null)
        {
            await RespondInteraction(
                embedBuilder: new EmbedBuilder()
                    .WithTitle("You're not in a voice channel!")
                    .WithDescription("To be able to create a voice channel, you have to be in one first. " +
                                     "This is so we can drag you in. ")
                    .WithColor(Color.Red)
            );
            return;
        }
        
        IVoiceChannel waitingChannel = Context.Guild.VoiceChannels.FirstOrDefault(channel => channel.Name == config.WaitingVcName);

        if (waitingChannel is null)
        {
            waitingChannel = await Context.Guild.CreateVoiceChannelAsync(config.WaitingVcName);

            await waitingChannel.ModifyAsync(properties => properties.CategoryId = config.PrivateCategoryId);

            await waitingChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, OverwritePermissions.DenyAll(waitingChannel));

            foreach (var role in config.AllowedRoles.Select(Context.Guild.GetRole).Where(r => r != null))
                await waitingChannel.AddPermissionOverwriteAsync(role, OverwritePermissions.DenyAll(waitingChannel).Modify(viewChannel: PermValue.Allow, connect: PermValue.Allow));

            foreach (var role in config.CreatorRoles.Select(Context.Guild.GetRole).Where(r => r != null))
                await waitingChannel.AddPermissionOverwriteAsync(role, OverwritePermissions.DenyAll(waitingChannel).Modify(viewChannel: PermValue.Allow, connect: PermValue.Allow, moveMembers: PermValue.Allow));

            foreach (var staffRole in GuildConfig.ModRoles.Concat(GuildConfig.AdminRoles).Select(Context.Guild.GetRole))
                await waitingChannel.AddPermissionOverwriteAsync(staffRole, OverwritePermissions.InheritAll.Modify(manageChannel: PermValue.Allow, viewChannel: PermValue.Allow, connect: PermValue.Allow, speak: PermValue.Allow, muteMembers: PermValue.Allow, deafenMembers: PermValue.Allow, moveMembers: PermValue.Allow, useVoiceActivation: PermValue.Allow));
        }

        IVoiceChannel newChannel = await Context.Guild.CreateVoiceChannelAsync(vcName);

        PermissionChecker.AddNewPrivateVc(newChannel.Id, Context.User.Id);

        await newChannel.ModifyAsync(properties => properties.CategoryId = config.PrivateCategoryId);

        await newChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, OverwritePermissions.DenyAll(waitingChannel));

        await newChannel.AddPermissionOverwriteAsync(Context.User,
            OverwritePermissions.AllowAll(newChannel).Modify(
                prioritySpeaker: PermValue.Deny,
                createInstantInvite: PermValue.Deny,
                mentionEveryone: PermValue.Deny
            )
        );

        foreach (var role in config.AllowedRoles.Select(Context.Guild.GetRole).Where(r => r != null))
        {
            await newChannel.AddPermissionOverwriteAsync(role,
                OverwritePermissions.DenyAll(newChannel).Modify(
                    speak: PermValue.Allow,
                    useVoiceActivation: PermValue.Allow,
                    stream: PermValue.Allow
                )
            );
        }

        foreach (var staffRole in GuildConfig.ModRoles.Concat(GuildConfig.AdminRoles).Select(Context.Guild.GetRole))
        {
            await newChannel.AddPermissionOverwriteAsync(staffRole,
                OverwritePermissions.InheritAll.Modify(
                    manageChannel: PermValue.Allow,
                    viewChannel: PermValue.Allow,
                    connect: PermValue.Allow,
                    speak: PermValue.Allow,
                    muteMembers: PermValue.Allow,
                    deafenMembers: PermValue.Allow,
                    moveMembers: PermValue.Allow,
                    useVoiceActivation: PermValue.Allow
                )
            );
        }
        
        await user.ModifyAsync(properties => properties.Channel = new Optional<IVoiceChannel>(newChannel));

        await RespondInteraction(
            embedBuilder: new EmbedBuilder()
                .WithTitle($"Created \"{vcName}\"")
                .WithDescription("Your private voice channel has successfully been created. " +
                                 "You should have full permission to edit it, move members and much more! " +
                                 "Have fun~!\n" +
                                 "**Note: You may not modify permissions to prevent staff from viewing or accessing the channel**, " +
                                 "as this interferes with moderation and monitoring abilities for things like xp grinding in voice calls.")
                .WithColor(Color.Green)
        );
    }
}
