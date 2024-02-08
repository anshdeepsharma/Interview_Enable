// <copyright file="AADAuthenticationHandler.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace MSX.Transition.API.Handler
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using MSX.Transition.API.Constants;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    public class AadAuthenticationHandler : AuthenticationHandler<AadAuthenticationOptions>
    {


        public AadAuthenticationHandler(IOptionsMonitor<AadAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task InitializeHandlerAsync()
        {
            return base.InitializeHandlerAsync();
        }

        /// <summary>
        /// Searches the 'Authorization' header for a 'Bearer' token. If the 'Bearer' token is found, it is validated using <see cref="TokenValidationParameters"/> set in the options.
        /// </summary>
        /// <returns>The Authentication Result.</returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string token = null;
            try
            {
                this.Logger.LogInformation($"Sending HandleAuthenticateAsync method request");
                string authHeader = this.Request.Headers[AuthConstants.AuthorizationHeaderKey];

                if (string.IsNullOrEmpty(authHeader))
                {
                    token = this.Request.Headers[AuthConstants.AadUserTicketHeaderKey];
                }
                else
                {
                    string[] bearer = authHeader.Split(' ');
                    if (bearer[0].Equals(AuthConstants.AuthorizationBearerKey))
                    {
                        token = bearer[1];
                    }
                }

                // If no token found, no further work possible
                if (string.IsNullOrEmpty(token))
                {
                    return AuthenticateResult.NoResult();
                }

                var validationParameters = this.Options.TokenValidationParameters.Clone();

                SecurityToken validatedToken;
                var validator = new JwtSecurityTokenHandler();
                if (validator.CanReadToken(token))
                {
                    ClaimsPrincipal principal;
                    try
                    {
                        principal = validator.ValidateToken(token, validationParameters, out validatedToken);

                        // now we verify other claims like appid, appidacr, ver, oid and tid

                        // initialize new list of extra claims
                        List<Claim> extraClaims = new List<Claim>();

                        // save raw token in list of extra claims
                        extraClaims.Add(new Claim("raw_token", token, ClaimValueTypes.Base64Binary));

                        // verify existance of TenantId claim
                        if (principal.FindFirst(AuthConstants.AadUserTicketClaimKeys.TenantIdClaimKeyOfficial) == null && principal.FindFirst(AuthConstants.AadUserTicketClaimKeys.TenantIdClaimKey) == null)
                        {
                            this.Logger.LogError("TenantId claim not found");
                            throw new Exception("TenantId claim not found");
                        }

                        // verify existance of appid claim
                        Claim appIdClaim = principal.FindFirst(AuthConstants.AadUserTicketClaimKeys.ApplicationId);
                        if (appIdClaim == null)
                        {
                            this.Logger.LogError("AppId claim not found");
                            throw new Exception("AppId claim not found");
                        }

                        // verify if the application is within list of allowed list applications
                        if (!this.Options.ValidAadApplications.Contains(appIdClaim.Value))
                        {
                            this.Logger.LogError("Application claim not found");
                            throw new Exception("Application not authorized");
                        }

                        // verify existance of scope claim
                        if (this.Options.isUserAuthentication)
                        {
                            Claim scopeClaim = principal.FindFirst(AuthConstants.AadUserTicketClaimKeys.Scope);
                            if (principal.FindFirst(AuthConstants.AadUserTicketClaimKeys.Scope) == null && principal.FindFirst(AuthConstants.AadUserTicketClaimKeys.Scp) == null)
                            {
                                this.Logger.LogError("Scope claim not found");
                                throw new Exception("Scope claim not found");
                            }
                        }

                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(principal.Identity, extraClaims);

                        var newprincipal = new ClaimsPrincipal(claimsIdentity);

                        var tokenValidatedContext = new TokenValidatedContext(this.Context, this.Scheme, this.Options)
                        {
                            Principal = newprincipal,
                            SecurityToken = validatedToken,
                        };

                        tokenValidatedContext.Success();
                        this.Logger.LogInformation($"HandleAuthenticateAsync method succeeded-tokenValidatedContext:{tokenValidatedContext.Result}");
                        return tokenValidatedContext.Result;
                    }
                    catch (Exception ex)
                    {
                        this.Logger.LogError(ex, "AAD Authentication Failed");
                        return AuthenticateResult.Fail(ex);
                    }
                }

                // Defect : CS0168 Fix - Async method lacks await operator for method HandleAuthenticateAsync
                await Task.Delay(0);
                return AuthenticateResult.Fail("No SecurityTokenValidator available for token: " + token ?? "[null]");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "ErrorProcessingMessage");

                throw;
            }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (this.Context.Request.Path.Value.ToLower().StartsWith("/api"))
            {
                await base.HandleChallengeAsync(properties);
            }
            else
            {
                await base.HandleForbiddenAsync(properties);
            }
        }

    }
}