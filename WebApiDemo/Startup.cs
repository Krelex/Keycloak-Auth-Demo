using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace WebApiDemo
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
			string authority = Configuration.GetValue<string>("Authority");

			services.AddControllers();

			services.AddAuthentication(options => {
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(cfg =>
			{
				// only for testing
				cfg.RequireHttpsMetadata = false;
				cfg.Authority = authority;
				cfg.IncludeErrorDetails = true;
				cfg.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateAudience = false,
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidIssuer = authority,
					ValidateLifetime = true
				};

				cfg.Events = new JwtBearerEvents()
				{
					OnAuthenticationFailed = c =>
					{
						c.NoResult();
						c.Response.StatusCode = 401;
						c.Response.ContentType = "text/plain";
						return c.Response.WriteAsync(c.Exception.ToString());
					}
				};
			});

			services.AddAuthorization(options =>
			   {
				   options.AddPolicy("ApiUser", policy => policy.RequireClaim("user_roles", "apiuser", "apiAdmin"));
				   options.AddPolicy("ApiAdmin", policy => policy.RequireClaim("user_roles", "apiAdmin"));
			   }
			);

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseAuthentication();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
