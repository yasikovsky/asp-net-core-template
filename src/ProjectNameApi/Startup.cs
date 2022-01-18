using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjectNameApi.Config;
using ProjectNameApi.Enums;
using ProjectNameApi.Helpers;
using ProjectNameApi.Services;

namespace ProjectNameApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureJsonSerializer(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(a =>
            {
                a.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                a.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                a.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
            });
        }

        public void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
        }

        public void ConfigureJwt(IServiceCollection services)
        {
            var secretConfig = Configuration.GetSection("SecretConfig").Get<SecretConfig>();
            var secretBytes = Encoding.ASCII.GetBytes(secretConfig.JwtAuthSecret);
            
            services.AddAuthentication(a =>
            {
                a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(a =>
            {
                a.RequireHttpsMetadata = false;
                a.SaveToken = true;
                a.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretBytes),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2)
                };
                
            });
        }

        public void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(a =>
            {
                a.SwaggerDoc("v1",
                    new OpenApiInfo {Title = "ProjectName API", Version = "v1"});

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                a.IncludeXmlComments(xmlPath);
                a.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme,<br /><br />Please enter into field the " +
                        "word 'Bearer' following by space and the access token.<br/><br/>Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                a.OperationFilter<SwaggerAuthFilter>();
                a.OperationFilter<SwaggerRoleFilter>();
                a.OperationFilter<SwaggerClaimFilter>();
            });
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureJsonSerializer(services);
            ConfigureCors(services);
            ConfigureJwt(services);
            ConfigureSwagger(services);
            ConfigureDependencies(services);

            TypeHandlerConfig.AddSqlTypeHandlers();

            // Suppress native model validation filters to allow custom model validation results
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
        }

        public void ConfigureDependencies(IServiceCollection services)
        {
            services.AddTransient<DbGateway>();
            services.AddTransient<ExampleService>();
            
            services.Configure<SecretConfig>(Configuration.GetSection("SecretConfig"));
            
            services.AddMemoryCache();
            services.AddAutoMapper(typeof(Startup));
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            app.UseCors("AllowAllOrigins");

            app.UseSwagger();
            app.UseSwaggerUI(a => { a.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectName API"); });

            ApiHelper.Environment = env.EnvironmentName.ToLower() switch
            {
                "dev" or "development" => EnvironmentType.Development,
                "beta" => EnvironmentType.Beta,
                "staging" => EnvironmentType.Staging,
                _ => EnvironmentType.Production
            };

            if (ApiHelper.Environment != EnvironmentType.Production)
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}