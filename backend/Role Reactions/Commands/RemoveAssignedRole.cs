﻿using Bot.Attributes;
using Bot.Enums;
using Bot.Translators;
using Discord;
using Discord.Interactions;
using RoleReactions.Abstractions;
using RoleReactions.Data;
using System.Runtime.InteropServices;

namespace RoleReactions.Commands;

public class RemoveAssignedRole : RoleMenuCommand<RemoveAssignedRole>
{
    public RoleReactionsDatabase Database { get; set; }

    [SlashCommand("remove-rm-role", "Removes a role to a role menu")]
    [Require(RequireCheck.GuildAdmin)]
    public async Task RemoveAssignedRoleCommand([Autocomplete(typeof(MenuHandler))] int menuId, IRole role,
        [Optional] ITextChannel channel)
    {
        if (channel == null)
            if (Context.Channel is ITextChannel txtChannel)
                channel = txtChannel;

        if (channel != null)
        {
            var menu = Database.RoleReactionsMenu.Find(channel.GuildId, channel.Id, menuId);

            if (menu == null)
            {
                await RespondInteraction($"Role menu `{menuId}` does not exist in this channel!");
                return;
            }

            if (menu.RoleToEmote.ContainsKey(role.Id))
            {
                await RespondInteraction($"Role `{role.Name}` already exists for role menu `{menu.Name}`!");
                return;
            }

            var message = await channel.GetMessageAsync(menu.MessageId);

            if (message == null)
            {
                await RespondInteraction($"Role menu `{menu.Name}` does not have a message related to it! " +
                    $"Please delete and recreate the menu.");
                return;
            }

            if (message is IUserMessage userMessage)
            {
                var rows = new List<Dictionary<ulong, string>>();
                var tempComp = new Dictionary<ulong, string>();

                foreach (var storeRole in menu.RoleToEmote)
                {
                    if (storeRole.Key == role.Id)
                        continue;

                    tempComp.Add(storeRole.Key, storeRole.Value);

                    if (tempComp.Count >= 5)
                        rows.Add(tempComp);
                }

                rows.Add(tempComp);

                var components = new ComponentBuilder();

                foreach (var row in rows)
                {
                    var aRow = new ActionRowBuilder();

                    foreach (var col in row)
                    {
                        IEmote intEmote = null;

                        if (Emote.TryParse(col.Value, out var pEmote))
                            intEmote = pEmote;

                        var intRole = Context.Guild.GetRole(col.Key);

                        aRow.WithButton(intRole.Name, $"add-rm-role:{intRole.Id},{Context.User.Id}", emote: intEmote);
                    }

                    components.AddRow(aRow);
                }

                await userMessage.ModifyAsync(m => m.Components = components.Build());

                menu.RoleToEmote.Remove(role.Id);

                await Database.SaveChangesAsync();

                await RespondInteraction($"Successfully removed role `{role.Name}` from menu `{menu.Name}`!");
            }
            else
            {
                await RespondInteraction($"Message for role menu `{menu.Name}` was not created by me! " +
                    $"Please delete and recreate the menu.");
                return;
            }
        }
        else
        {
            await RespondInteraction(Translator.Get<BotTranslator>().OnlyTextChannel());
            return;
        }
    }
}
