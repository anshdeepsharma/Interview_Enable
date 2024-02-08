using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MSX.Account.API;
using MSX.Transition.API.Handler;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace MSX.Transition.API.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static ConcurrentDictionary<string, ConfigurationManager<OpenIdConnectConfiguration>> configManagerSigningKeysDictionary = new ConcurrentDictionary<string, ConfigurationManager<OpenIdConnectConfiguration>>();

        public static void RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var authorities = configuration["Account:Authentication:Authority"];
            var issuer = configuration["Account:Authentication:Issuer"];
            var audiences = configuration["Account:Authentication:Audiences"];

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.Authority = authorities;//"https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47";
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidIssuer = issuer,//"https://sts.windows.net/72f988bf-86f1-41af-91ab-2d7cd011db47/",
            //            ValidateAudience = true,
            //            ValidAudiences = audiences?.Split(",").ToList(),//new List<string>() { "4b010c67-2680-414a-857f-1a5e714fe1a4", "https://localhost:10746" },
            //            ValidateLifetime = true
            //        };
            //        options.Events = new JwtBearerEvents
            //        {
            //            OnAuthenticationFailed = context =>
            //            {
            //                // Handle authentication failure
            //                return Task.CompletedTask;
            //            },
            //            OnTokenValidated = context =>
            //            {
            //                // Access the authenticated user claims
            //                // You can use context.Principal to access the claims
            //                return Task.CompletedTask;
            //            }
            //        };
            //    })
            services.AddAuthentication()
           .AddScheme<AadAuthenticationOptions, AadAuthenticationHandler>("User", options =>
           {
               GetAuthenticationOptions(options, configuration);
           })
           .AddScheme<AadAuthenticationOptions, AadAuthenticationHandler>("STS", options =>
           {
               GetAuthenticationOptions(options, configuration);
               options.isUserAuthentication = false;
           });
        }

        public static void RegisterAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy =>
                {
                    policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                    (c.Type == "http://schemas.microsoft.com/identity/claims/scope" || c.Type == "scp") &&
                            c.Value.Contains("user_impersonation")));
                    policy.AddAuthenticationSchemes("User");
                    policy.RequireAuthenticatedUser();
                });
                options.AddPolicy("STS", policy =>
                {
                    policy.RequireClaim("appid");
                    policy.AddAuthenticationSchemes("STS");
                    policy.RequireAuthenticatedUser();
                });
            });
        }

        private static AadAuthenticationOptions GetAuthenticationOptions(AadAuthenticationOptions options
            , IConfiguration configuration)
        {

            var aadAuthorities = configuration["Account:Authentication:Authority"]?.Split(",").ToList();
            var issuers = configuration["Account:Authentication:Issuer"]?.Split(",").ToList();
            var audiences = configuration["Account:Authentication:Audiences"]?.Split(",").ToList();
            var validAadApplications = configuration["Account:Authentication:ValidAadApplications"]?.Split(",").ToList();

            List<string> authorities = aadAuthorities!;
            /*List<SecurityKey> allSigningKeys = new List<SecurityKey>();
            foreach (var aadAuthority in authorities)
            {
                var retTuple = GetSigningKeysForAuthority(aadAuthority);
                var signingKeys = retTuple.Item1;
                var issuer = retTuple.Item2;
                allSigningKeys.AddRange(signingKeys);
                issuers!.Add(issuer);
            }*/


            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                IssuerValidator = AADIssuerValidator.Validate,
                ValidIssuers = issuers,
                ValidAudiences = audiences ?? new List<string>(),
                RequireExpirationTime = true,
                RequireAudience = true,
                SignatureValidator = AADIssuerValidator.SignatureValidtor   // another approach when GetSigningKeys from open id does not work
                //IssuerSigningKeys = allSigningKeys,
                //ValidateIssuerSigningKey = false,
            };

            options.ValidAadApplications = validAadApplications!;
            options.isUserAuthentication = true;

            return options;
        }


        private static Tuple<List<SecurityKey>, string> GetSigningKeysForAuthority(string aadAuthority)
        {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Host = aadAuthority;
            uriBuilder.Path = "/common/v2.0/.well-known/openid-configuration";
            uriBuilder.Scheme = "https";

            ConfigurationManager<OpenIdConnectConfiguration> configManager = null;

            if (configManagerSigningKeysDictionary.ContainsKey(uriBuilder.Uri.AbsoluteUri))
            {
                configManager = configManagerSigningKeysDictionary[uriBuilder.Uri.AbsoluteUri];
            }
            else
            {
                configManager = new ConfigurationManager<OpenIdConnectConfiguration>(uriBuilder.Uri.AbsoluteUri, new OpenIdConnectConfigurationRetriever());
                configManagerSigningKeysDictionary.GetOrAdd(uriBuilder.Uri.AbsoluteUri, configManager);
            }


            var openidconfig = configManager.GetConfigurationAsync().Result;
            return Tuple.Create(openidconfig.SigningKeys.ToList(), openidconfig.Issuer);
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition(
                        "token",
                        new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.Http,
                            BearerFormat = "JWT",
                            Scheme = "Bearer",
                            In = ParameterLocation.Header,
                            Name = HeaderNames.Authorization
                        }
                    );
                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "token"
                                },
                            },
                            Array.Empty<string>()
                        }
                    });
            });

            services.ConfigureOptions<ConfigureSwaggerOptions>();

        }
    }
}
