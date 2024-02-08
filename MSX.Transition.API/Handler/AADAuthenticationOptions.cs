// <copyright file="AADAuthenticationOptions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace MSX.Transition.API.Handler
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;

    public class AadAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets or sets the parameters used to validate identity tokens.
        /// </summary>
        /// <remarks>Contains the types and definitions required for validating a token.</remarks>
        /// <exception cref="ArgumentNullException">if 'value' is null.</exception>
        /// <value>
        /// The parameters used to validate identity tokens.
        /// </value>
        public TokenValidationParameters TokenValidationParameters { get; set; } = new TokenValidationParameters();

        public IEnumerable<string> ValidAadApplications { get; set; } = new List<string>();

        public bool isUserAuthentication { get; set; }
    }

}
