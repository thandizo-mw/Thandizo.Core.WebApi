using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.System.Text.Json;
using System;
using System.IO;
using Thandizo.DAL.Models;

namespace Thandizo.Core.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var nationalStatisticsUrl = Configuration["LocalStatisticsEndpoint"];
            services.AddControllers();
            services.AddEntityFrameworkNpgsql().AddDbContext<thandizoContext>(options =>
                        options.UseNpgsql(Configuration.GetConnectionString("DatabaseConnection")));
            services.AddDomainServices(nationalStatisticsUrl);

            //Disable automatic model state validation to provide cleaner error messages to avoid default complex object
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Thandizo Core API",
                    Description = "Web API for Thandizo platform",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact { Name = "COVID-19 Malawi Tech Response", Email = "thandizo.mw@gmail.com", Url = new Uri("https://www.thandizo.mw") }
                });
                c.IncludeXmlComments(GetXmlCommentsPath());
            });

            var redisHost = Configuration["RedisHost"];
            

            var conf = new RedisConfiguration
            {
                AbortOnConnectFail = true,
                Hosts = new RedisHost[]
                {
                    new RedisHost { Host = redisHost, Port = 6379 }
                },             
                AllowAdmin = true,
                ConnectTimeout = 3000,
                Database = 0,
                ServerEnumerationStrategy = new ServerEnumerationStrategy()
                {
                    Mode = ServerEnumerationStrategy.ModeOptions.All,
                    TargetRole = ServerEnumerationStrategy.TargetRoleOptions.Any,
                    UnreachableServerAction = ServerEnumerationStrategy.UnreachableServerActionOptions.Throw
                }
            };

            services.AddStackExchangeRedisExtensions<SystemTextJsonSerializer>(conf);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //this is not needed in PRODUCTION but only in Hosted Testing Environment
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Thandizo Core API V1");
            });
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod());
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UserRedisInformation();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private string GetXmlCommentsPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Thandizo.Core.WebApi.xml");
        }
    }
}
