﻿using AspNetCoreRateLimit;
using MASZ.Bot.Abstractions;
using MASZ.Bot.Middleware;
using MASZ.Bot.Models;
using MASZ.Bot.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MASZ.Bot;

public class BotWebModule : WebModule
{
	public override string Maintainer => "Zaanposni";

	public override string[] Contributors { get; } = { "Ferox" };

	public override string[] Translators { get; } = { "Bricksmaster", "FlixProd" };

	public override string[] AddAuthorizationPolicy()
	{
		return new[] { "Cookies", "Tokens" };
	}

	public override void ConfigureServices(ConfigurationManager configuration, IServiceCollection services)
	{
		services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

		services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));
	}

	public override void AddServices(IServiceCollection services, ServiceCacher serviceCacher, AppSettings settings)
	{
		services.AddSingleton<FilesHandler>();

		services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie("Cookies", options =>
			{
				options.LoginPath = "/api/v1/login";
				options.LogoutPath = "/api/v1/logout";
				options.ExpireTimeSpan = new TimeSpan(7, 0, 0, 0);
				options.Cookie.MaxAge = new TimeSpan(7, 0, 0, 0);
				options.Cookie.Name = "masz_access_token";
				options.Cookie.HttpOnly = false;
				options.Events.OnRedirectToLogin = context =>
				{
					context.Response.Headers["Location"] = context.RedirectUri;
					context.Response.StatusCode = 401;
					return Task.CompletedTask;
				};
			})
			.AddDiscord(options =>
			{
				options.ClientId = settings.ClientId.ToString();
				options.ClientSecret = settings.ClientSecret;
				options.Scope.Add("guilds");
				options.Scope.Add("identify");
				options.SaveTokens = true;
				options.Prompt = "none";
				options.AccessDeniedPath = "/oauthfailed";
				options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.CorrelationCookie.SameSite = SameSiteMode.Lax;
				options.CorrelationCookie.HttpOnly = false;
			});

		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer("Tokens", x =>
			{
				x.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.DiscordBotToken)),
					ValidateIssuer = false,
					ValidateAudience = false
				};
			});

		if (settings.CorsEnabled)
			services.AddCors(o => o.AddPolicy("AngularDevCors", builder =>
			{
				builder.WithOrigins("http://127.0.0.1:4200")
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials();
			}));

		services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
		services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
		services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
		services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
	}

	public override void PostWebBuild(WebApplication app, AppSettings settings)
	{
		if (settings.CorsEnabled)
			app.UseCors("AngularDevCors");

		if (app.Environment.IsDevelopment())
			app.UseDeveloperExceptionPage();

		app.UseIpRateLimiting();

		app.UseMiddleware<HeaderMiddleware>();
		app.UseMiddleware<RequestLoggingMiddleware>();
		app.UseMiddleware<ApiExceptionHandlingMiddleware>();
	}
}