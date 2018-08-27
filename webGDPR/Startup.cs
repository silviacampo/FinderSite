using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webGDPR.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using webGDPR.Infrastructure;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using webGDPR.Authorization;
using AgendaSignalR.Infrastructure;
using System;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;

namespace webGDPR
{
	public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

			services.AddIdentity<ApplicationUser, IdentityRole>(options => options.Stores.MaxLengthForKeys = 128)
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultUI()
			.AddDefaultTokenProviders();

			//to generate identity pages
			// dotnet tool install--global dotnet-aspnet - codegenerator
			// in powershell:
			//dotnet aspnet-codegenerator identity - dc webGDPR.Data.ApplicationDbContext --force

			services.AddMvc(options =>
			{
				options.Filters.Add(new HostFilter());
			}).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			services.AddScoped<HostFilter>();
			services.AddTransient<IEmailSender, EmailSender>();

			// Authorization handlers.
			services.AddScoped<IAuthorizationHandler, UserIsOwnerAuthorizationHandler>();
			services.AddSingleton<IAuthorizationHandler, UserAdministratorsAuthorizationHandler>();
			services.AddSingleton<IAuthorizationHandler, UserManagerAuthorizationHandler>();

			services.AddSingleton<ICustomWebSocketFactory, CustomWebSocketFactory>();
			services.AddSingleton<ICustomWebSocketMessageHandler, CustomWebSocketMessageHandler>();

		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			/* https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-apache?view=aspnetcore-2.1&tabs=aspnetcore2x
			 * at the top, in any or all files
			 <VirtualHost *:*>
    RequestHeader set "X-Forwarded-Proto" expr=%{REQUEST_SCHEME}
</VirtualHost>

			<VirtualHost *:443>
    ProxyPreserveHost On
    ProxyPass / http://127.0.0.1:8060/
    ProxyPassReverse / http://127.0.0.1:8060/
    ServerName test.whereisfinder.com
    ServerAlias test.whereisfinder.com
    ErrorLog ${APACHE_LOG_DIR}testfinder-error.log
    CustomLog ${APACHE_LOG_DIR}testfinder-access.log common
</VirtualHost>

			[Unit]
Description=Test Finder

[Service]
WorkingDirectory=/var/www/finder
ExecStart=/usr/local/bin/dotnet /var/www/finder/publish/finder.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
SyslogIdentifier=dotnet-finder
User=apache
Environment=ASPNETCORE_ENVIRONMENT=Production 

[Install]
WantedBy=multi-user.target
			 */

			loggerFactory.AddLog4Net();

			app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

			var webSocketOptions = new WebSocketOptions()
			{
				KeepAliveInterval = TimeSpan.FromSeconds(120),
				ReceiveBufferSize = 4 * 1024
			};
			app.UseWebSockets(webSocketOptions);

			app.UseCustomWebSocketManager();

			app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
