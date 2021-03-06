﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SlackFilter.Configuration;
using SlackFilter.MessageProcessor;
using SlackFilter.ServiceClients;
using Spin.WebApi;
using Swashbuckle.AspNetCore.Swagger;

namespace SlackFilter
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(Factories.ExecutionContextFactory);

            services.AddSpinRequestLogging()
                .AddSpinLogger();

            var configuration = Configuration.GetSection("SlackFilterConfiguration").Get<SlackFilterConfiguration>();

            if (Configuration["PersonalToken"] != null)
                configuration.PersonalToken = Configuration["PersonalToken"];

            services.AddSingleton(configuration);

            services.AddScoped<SlackMessageProcessor>();
            services.AddScoped<VstsClient>();

            
            services.AddMvc()
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.Converters.Add(new StringEnumConverter());
                    opt.SerializerSettings.Formatting = Formatting.Indented;
                    opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Version = "v1", Title = "SlackFilterWebApi" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSpinRequestLogging();

            app.UseSpinExecutionContextPopulatorMiddleware()
                .AlwaysUseCachedSpinRequestContextData();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", env.ApplicationName);
            });

            app.UseMvc();
        }
    }
}
