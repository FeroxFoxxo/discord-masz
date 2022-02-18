using Bot.Abstractions;
using Bot.Data;
using Bot.Enums;
using Bot.Events;
using Bot.Extensions;
using Bot.Models;
using Bot.Translators;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bot.Services;

public class DiscordBot : IHostedService, Event
{
	private readonly DiscordSocketClient _client;
	private readonly BotEventHandler _eventHandler;
	private readonly InteractionService _interactions;
	private readonly CachedServices _cacher;
	private readonly ILogger<DiscordBot> _logger;
	private readonly IServiceProvider _serviceProvider;

	private bool _firstReady;
	private bool _isRunning;
	private DateTime? _lastDisconnect;

	public DiscordBot(ILogger<DiscordBot> logger, DiscordSocketClient client, InteractionService interactions,
		IServiceProvider serviceProvider, BotEventHandler eventHandler, CachedServices cacher)
	{
		_logger = logger;
		_client = client;
		_interactions = interactions;
		_serviceProvider = serviceProvider;
		_eventHandler = eventHandler;
		_cacher = cacher;

		_firstReady = true;
		_isRunning = false;
		_lastDisconnect = DateTime.UtcNow;
	}

	public void RegisterEvents()
	{
		_client.JoinedGuild += GuildCreatedHandler;
		_client.GuildMemberUpdated += GuildUserUpdatedHandler;
		_client.UserLeft += GuildUserRemoved;
		_client.UserBanned += GuildBanAdded;
		_client.UserUnbanned += GuildBanRemoved;
		_client.ThreadCreated += ThreadCreatedHandler;

		_client.Connected += Connected;
		_client.Disconnected += Disconnected;
		_client.Ready += ReadyHandler;

		_client.InteractionCreated += HandleInteraction;

		_interactions.SlashCommandExecuted += CmdErrorHandler;

		var clientLogger = _serviceProvider.GetRequiredService<ILogger<DiscordSocketClient>>();

		_client.Log += logLevel => Log(logLevel, clientLogger);

		_client.JoinedGuild += JoinGuild;

		var interactionsLogger = _serviceProvider.GetRequiredService<ILogger<InteractionService>>();

		_interactions.Log += logLevel => Log(logLevel, interactionsLogger);
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		using var scope = _serviceProvider.CreateScope();

		try
		{
			foreach (var assembly in _cacher.Dependents)
				await _interactions.AddModulesAsync(assembly, scope.ServiceProvider);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Modules could not initialize!");
			return;
		}

		var settingsRepository = scope.ServiceProvider.GetRequiredService<SettingsRepository>();

		var config = await settingsRepository.GetAppSettings();

		await _client.LoginAsync(TokenType.Bot, config.DiscordBotToken);
		await _client.StartAsync();
		await _client.SetGameAsync(config.ServiceBaseUrl, type: ActivityType.Watching);
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		await _client.LogoutAsync();
	}

	private async Task HandleInteraction(SocketInteraction arg)
	{
		try
		{
			var ctx = new SocketInteractionContext(_client, arg);

			await _interactions.ExecuteCommandAsync(ctx, _serviceProvider);
		}
		catch (Exception)
		{
			Console.WriteLine($"Unable to execute {arg.Type} in channel {arg.Channel}");

			if (arg.Type is InteractionType.ApplicationCommand)
				await arg.GetOriginalResponseAsync().ContinueWith(async msg => await msg.Result.DeleteAsync());
		}
	}

	public bool IsRunning()
	{
		return _isRunning;
	}

	public DateTime? GetLastDisconnectTime()
	{
		return _lastDisconnect;
	}

	public int GetLatency()
	{
		return _client.Latency;
	}

	private Task Connected()
	{
		_logger.LogCritical("Client connected.");
		_isRunning = true;

		return Task.CompletedTask;
	}

	private Task Disconnected(Exception _)
	{
		_logger.LogCritical("Client disconnected.");
		_isRunning = false;
		_lastDisconnect = DateTime.UtcNow;

		return Task.CompletedTask;
	}

	private async Task ReadyHandler()
	{
		_logger.LogInformation("Client connected.");
		_isRunning = true;

		try
		{
			await _client.BulkOverwriteGlobalApplicationCommandsAsync(Array.Empty<ApplicationCommandProperties>());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Something went wrong while overwriting global application commands.");
		}

		foreach (var guild in _client.Guilds)
			try
			{
				await JoinGuild(guild);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Something went wrong while handling guild join for {guild.Id}.");
			}

		if (_firstReady)
		{
			_firstReady = false;

			_eventHandler.BotLaunchedEvent.Invoke();
		}
	}

	private static Task Log(LogMessage logMessage, ILogger logger)
	{
		var level = logMessage.Severity switch
		{
			LogSeverity.Info => LogLevel.Information,
			LogSeverity.Debug => LogLevel.Debug,
			LogSeverity.Critical => LogLevel.Critical,
			LogSeverity.Error => LogLevel.Error,
			LogSeverity.Verbose => LogLevel.Trace,
			LogSeverity.Warning => LogLevel.Warning,
			_ => throw new NotImplementedException()
		};

		if (logMessage.Exception is null)
			logger.Log(level, logMessage.Message);
		else
			logger.LogError(logMessage.Exception, logMessage.Message);

		return Task.CompletedTask;
	}

	private async Task JoinGuild(SocketGuild guild)
	{
		await _interactions.RegisterCommandsToGuildAsync(
			guild.Id
		);

		_logger.LogInformation($"Initialized guild commands for guild {guild.Name}.");
	}

	private Task GuildBanRemoved(SocketUser user, SocketGuild guild)
	{
		using var scope = _serviceProvider.CreateScope();

		// Refresh ban cache
		var discordRest = scope.ServiceProvider.GetRequiredService<DiscordRest>();
		discordRest.RemoveFromCache(CacheKey.GuildBan(guild.Id, user.Id));

		return Task.CompletedTask;
	}

	private async Task GuildBanAdded(SocketUser user, SocketGuild guild)
	{
		using var scope = _serviceProvider.CreateScope();

		// Refresh ban cache
		var discordRest = scope.ServiceProvider.GetRequiredService<DiscordRest>();
		await discordRest.GetGuildUserBan(guild.Id, user.Id, CacheBehavior.IgnoreCache);
		discordRest.RemoveFromCache(CacheKey.GuildUser(guild.Id, user.Id));

		// Refresh identity memberships
		var identityManager = scope.ServiceProvider.GetRequiredService<IdentityManager>();
		foreach (var identity in identityManager.GetCurrentIdentities()
					 .Where(identity => identity.GetCurrentUser().Id == user.Id))
			identity.RemoveGuildMembership(guild.Id);
	}

	private Task GuildUserRemoved(SocketGuild guild, SocketUser usr)
	{
		using var scope = _serviceProvider.CreateScope();

		// Refresh identity memberships
		var identityManager = scope.ServiceProvider.GetRequiredService<IdentityManager>();
		foreach (var identity in identityManager.GetCurrentIdentities()
					 .Where(identity => identity.GetCurrentUser().Id == usr.Id))
			identity.RemoveGuildMembership(guild.Id);

		return Task.CompletedTask;
	}

	private Task GuildUserUpdatedHandler(Cacheable<SocketGuildUser, ulong> oldUsrCached, SocketGuildUser newUsr)
	{
		using var scope = _serviceProvider.CreateScope();

		// Refresh identity memberships
		var identityManager = scope.ServiceProvider.GetRequiredService<IdentityManager>();
		foreach (var identity in identityManager.GetCurrentIdentities()
					 .Where(identity => identity.GetCurrentUser().Id == newUsr.Id))
			identity.UpdateGuildMembership(newUsr);

		// Refresh user cache
		var discordRest = scope.ServiceProvider.GetRequiredService<DiscordRest>();
		discordRest.AddOrUpdateCache(CacheKey.GuildUser(newUsr.Id, newUsr.Id), new CacheApiResponse(newUsr));

		return Task.CompletedTask;
	}

	private static async Task ThreadCreatedHandler(SocketThreadChannel channel)
	{
		await channel.JoinAsync();
	}

	private Task GuildCreatedHandler(SocketGuild guild)
	{
		_logger.LogInformation($"I joined guild '{guild.Name}' with ID: '{guild.Id}'");
		return Task.CompletedTask;
	}

	private async Task CmdErrorHandler(SlashCommandInfo info, IInteractionContext context, IResult result)
	{
		if (!result.IsSuccess)
		{
			if (result is ExecuteResult eResult)
			{
				if (eResult.Exception is ApiException exception)
				{
					_logger.LogError(
						$"Command '{info.Name}' invoked by '{context.User.Username}#{context.User.Discriminator}' failed: {exception.Error}");

					using var scope = _serviceProvider.CreateScope();
					var translator = scope.ServiceProvider.GetRequiredService<Translation>();

					if (context.Guild != null)
						await translator.SetLanguage(context.Guild.Id);

					var errorCode = "#" + ((int)exception.Error).ToString("D4");

					var builder = new EmbedBuilder()
						.WithTitle(translator.Get<BotTranslator>().SomethingWentWrong())
						.WithColor(Color.Red)
						.WithDescription(translator.Get<BotEnumTranslator>().Enum(exception.Error))
						.WithCurrentTimestamp()
						.WithFooter($"{translator.Get<BotTranslator>().Code()} {errorCode}");

					try
					{
						if (context is SocketInteraction interaction)
							if (!interaction.HasResponded)
								await interaction.RespondAsync(embed: builder.Build());
							else
								await context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());
						else
							await context.Channel.SendMessageAsync(embed: builder.Build());
					}
					catch (Exception)
					{
						await context.Channel.SendMessageAsync(embed: builder.Build());
					}
				}
				else
				{
					_logger.LogError(
						$"Command '{info.Name}' invoked by '{context.User.Username}#{context.User.Discriminator}' failed: " +
						eResult.Exception.Message + "\n" + eResult.Exception.StackTrace);
				}
			}
			else
			{
				_logger.LogError(
					$"Command '{info.Name}' invoked by '{context.User.Username}#{context.User.Discriminator}' failed due to {result.Error}.");
			}
		}
	}
}