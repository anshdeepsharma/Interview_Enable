// <copyright file="AADIssuerValidator.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace MSX.Transition.API.Handler
{
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.JsonWebTokens;
    using Microsoft.IdentityModel.Tokens;
    using MSX.Transition.API.Constants;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;

    [ExcludeFromCodeCoverage]
    public class AADIssuerValidator
    {
        private static ILogger<AADIssuerValidator> logger;
        static AADIssuerValidator()
        {
            logger = new LoggerFactory().CreateLogger<AADIssuerValidator>();
        }
        public static JwtSecurityToken SignatureValidtor(string token, TokenValidationParameters validationParameters)
        {
            return new JwtSecurityToken(token);
        }
        public static string Validate(string actualIssuer
            , SecurityToken securityToken
            , TokenValidationParameters validationParameters)
        {
            if (string.IsNullOrEmpty(actualIssuer))
            {
                logger.LogError("actualIssuer can't be null");
                throw new ArgumentNullException(nameof(actualIssuer));
            }

            if (securityToken == null)
            {
                logger.LogError("securityToken can't be null");
                throw new ArgumentNullException(nameof(securityToken));
            }

            if (validationParameters == null)
            {
                logger.LogError("validationParameters can't be null");
                throw new ArgumentNullException(nameof(validationParameters));
            }

            string tenantId = GetTenantIdFromToken(securityToken);
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                logger.LogError("tenantId can't be null");
                throw new SecurityTokenInvalidIssuerException("tenantId not present in token");
            }

            if (validationParameters.ValidIssuers != null)
            {
                foreach (var validIssuerTemplate in validationParameters.ValidIssuers)
                {
                    if (IsValidIssuer(validIssuerTemplate, tenantId, actualIssuer))
                    {
                        logger.LogInformation($"validation succeeded with user:{actualIssuer}");
                        return actualIssuer;
                    }
                }
            }

            if (IsValidIssuer(validationParameters.ValidIssuer, tenantId, actualIssuer))
            {
                logger.LogInformation($"validation succeeded with user:{actualIssuer}");
                return actualIssuer;
            }

            logger.LogError("failed due to Issuer {0} doesn't match list of valid issuersr", actualIssuer);

            // If a valid issuer is not found, throw
            throw new SecurityTokenInvalidIssuerException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Issuer {0} doesn't match list of valid issuers",
                    actualIssuer));
        }

        private static bool IsValidIssuer(string validIssuerTemplate, string tenantId, string actualIssuer)
        {
            if (string.IsNullOrEmpty(validIssuerTemplate))
            {
                logger.LogInformation("not valid Isusers returning false");
                return false;
            }

            try
            {
                Uri issuerFromTemplateUri = new Uri(validIssuerTemplate.Replace("{tenantid}", tenantId));
                Uri actualIssuerUri = new Uri(actualIssuer);

                // Template authority is equal to actual authority
                var v1 = issuerFromTemplateUri.Authority == actualIssuerUri.Authority;

                // Template authority ends in the tenant ID
                var v2 = IsValidTidInLocalPath(tenantId, issuerFromTemplateUri);

                // "iss" ends in the tenant ID
                var v3 = IsValidTidInLocalPath(tenantId, actualIssuerUri);
                logger.LogInformation($"IsValidIssuer method succeeded-{v1},{v2},{v3}");
                return v1 && v2 && v3;

                // Template authority is equal to actual authority
                // return issuerFromTemplateUri.Authority == actualIssuerUri.Authority &&

                // // Template authority ends in the tenant ID
                //      IsValidTidInLocalPath(tenantId, issuerFromTemplateUri) &&

                // // "iss" ends in the tenant ID
                //      IsValidTidInLocalPath(tenantId, actualIssuerUri);
            }
            catch
            {
                logger.LogError("failed in catch block of IsValidIssuer");// if something faults, ignore
            }

            logger.LogInformation("IsValidIssuer method returned false");
            return false;
        }

        private static bool IsValidTidInLocalPath(string tenantId, Uri uri)
        {
            string trimmedLocalPath = uri.LocalPath.Trim('/');
            return trimmedLocalPath == tenantId || trimmedLocalPath == $"{tenantId}/v2.0";
        }

        /// <summary>Gets the tenant ID from a token.</summary>
        /// <param name="securityToken">A JWT token.</param>
        /// <returns>A string containing the tenant ID, if found or <see cref="string.Empty"/>.</returns>
        /// <remarks>Only <see cref="JwtSecurityToken"/> and <see cref="JsonWebToken"/> are acceptable types.</remarks>
        private static string GetTenantIdFromToken(SecurityToken securityToken)
        {
            object tid;
            object tenantId;
            if (securityToken is JwtSecurityToken jwtSecurityToken)
            {
                logger.LogInformation("get JWt security token  value for TenantIdClaimKey");
                jwtSecurityToken.Payload.TryGetValue(AuthConstants.AadUserTicketClaimKeys.TenantIdClaimKey, out tid);
                if (tid != null)
                {
                    logger.LogInformation($"GetTenantIdFromToken method returned  jwt TenantIdClaimKey value  tid:{tid}");
                    return (string)tid;
                }

                logger.LogInformation("get JWt security token value for TenantIdClaimKeyOfficial");
                jwtSecurityToken.Payload.TryGetValue(AuthConstants.AadUserTicketClaimKeys.TenantIdClaimKeyOfficial, out tenantId);
                if (tenantId != null)
                {
                    logger.LogInformation($"GetTenantIdFromToken method returned  jwt TenantIdClaimKeyOfficial value  tenantId:{tenantId}");
                    return (string)tenantId;
                }
            }

            if (securityToken is JsonWebToken jsonWebToken)
            {
                logger.LogInformation("get json web token value for TenantIdClaimKey");
                jsonWebToken.TryGetPayloadValue(AuthConstants.AadUserTicketClaimKeys.TenantIdClaimKey, out tid);
                if (tid != null)
                {
                    logger.LogInformation($"GetTenantIdFromToken method returned  json TenantIdClaimKey value  tid:{tid}");
                    return (string)tid;
                }

                logger.LogInformation("get json web token value for TenantIdClaimKeyOfficial");
                jsonWebToken.TryGetPayloadValue(AuthConstants.AadUserTicketClaimKeys.TenantIdClaimKeyOfficial, out tenantId);
                if (tenantId != null)
                {
                    logger.LogInformation($"GetTenantIdFromToken method returned  json TenantIdClaimKeyOfficial value tenantId:{tenantId}");
                    return (string)tenantId;
                }
            }

            logger.LogInformation("GetTenantIdFromToken method returned with empty value");
            return string.Empty;
        }
    }
}
