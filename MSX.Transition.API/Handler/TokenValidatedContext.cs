// <copyright file="TokenValidatedContext.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace MSX.Transition.API.Handler
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.IdentityModel.Tokens;

    public class TokenValidatedContext : ResultContext<AadAuthenticationOptions>
    {
        public TokenValidatedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            AadAuthenticationOptions options)
            : base(context, scheme, options)
        {
        }

        public SecurityToken SecurityToken { get; set; }
    }
}