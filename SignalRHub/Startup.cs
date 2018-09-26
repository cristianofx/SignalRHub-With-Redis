using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using SignalRHub.Contracts;
using SignalRHub.Models.Config;
using SignalRHub.Models.Models;
using SignalRHub.SignalR;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Net.Http.Headers;

namespace SignalRHub
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration _configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCaching();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("Content-Disposition")
                    .Build());
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "SignalR Hub Test",
                        Version = "v1",
                        Description = "SignalR Hub Test",
                        TermsOfService = "None"
                    }
                );
                options.DescribeAllEnumsAsStrings();
                options.CustomSchemaIds(x => x.FullName);
            });


            services.AddOptions();

            var appSettings = new AppSettings();
            _configuration.Bind("Redis", appSettings.Redis);

            services.AddSignalR().AddRedis($"{appSettings.Redis.Host}:{appSettings.Redis.Port},password={appSettings.Redis.Password},defaultDatabase={Constants.RedisSignalRMainDatabase}");

            services.AddSingleton(appSettings);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<ISignalRMainHub, SignalRMainHub>();
            services.AddSingleton<HubSubscriptionInMemory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseResponseCaching();

            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(1)
                };
                context.Response.Headers[HeaderNames.CacheControl] = new string[] { "no-cache, no-store, must-revalidate" };
                context.Response.Headers[HeaderNames.Pragma] = new string[] { "no-cache" };
                context.Response.Headers[HeaderNames.Expires] = new string[] { "0" };

                await next();
            });

            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSignalR(routes =>
            {
                routes.MapHub<SignalRMainHub>("/hubs/echo");
            });

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "SignalR Hub Test"));
        }
    }
}
