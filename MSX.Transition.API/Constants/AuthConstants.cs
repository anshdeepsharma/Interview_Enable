// <copyright file="AuthConstants.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace MSX.Transition.API.Constants
{
    /// <summary>
    /// Class AuthConstants.
    /// </summary>
    public class AuthConstants
    {
        /// <summary>
        /// AadUserTicketHeaderKey.
        /// </summary>
        public const string AadUserTicketHeaderKey = "X-AadUserTicket";

        /// <summary>
        /// TenantHeaderKey.
        /// </summary>
        public const string TenantHeaderKey = "TenantHeader";

        /// <summary>
        /// ObjectHeaderKey.
        /// </summary>
        public const string ObjectHeaderKey = "ObjectHeader";

        /// <summary>
        /// AuthorizationBearerKey.
        /// </summary>
        public const string AuthorizationBearerKey = "Bearer";

        /// <summary>
        /// AuthorizationHeaderKey.
        /// </summary>
        public const string AuthorizationHeaderKey = "Authorization";

        /// <summary>
        /// key for account resource.
        /// </summary>
        public const string ResourceAccountKey = "account";

        /// <summary>
        /// key for read acccess for the resource.
        /// </summary>
        public const string ResourceReadAccessKey = "read";

        /// <summary>
        /// MSA User Ticket Claims Keys.
        /// </summary>
        public static class MSAUserTicketClaimKeys
        {
            /// <summary>
            /// STSUri claim: "idp".
            /// </summary>
            public const string STSUri = "idp";

            /// <summary>
            /// AltSecurityKey claim: "altsecid".
            /// </summary>
            public const string AltSecurityKey = "altsecid";
        }

        /// <summary>
        /// Azure Active Directory User Ticket Claims Keys.
        /// </summary>
        public static class AadUserTicketClaimKeys
        {
            /// <summary>
            /// Email claim: "email".
            /// </summary>
            public const string EmailClaimKey = "email";

            /// <summary>
            /// New UniqueName claim: "unique_name".
            /// </summary>
            public const string UniqueNameClaimKey = "unique_name";

            /// <summary>
            /// Old UniqueName claim: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name".
            /// </summary>
            public const string UniqueNameClaimKeyOfficial = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

            /// <summary>
            /// Name claim: "name".
            /// </summary>
            public const string NameClaimKey = "name";

            /// <summary>
            /// Old Object Id claim: http://schemas.microsoft.com/identity/claims/objectidentifier.
            /// </summary>
            public const string ObjectIdClaimKeyOfficial = "http://schemas.microsoft.com/identity/claims/objectidentifier";

            /// <summary>
            /// New Object id claim: "oid".
            /// </summary>
            public const string ObjectIdClaimKey = "oid";

            /// <summary>
            /// PreferredUserName: "preferred_username".
            /// </summary>
            public const string PreferredUserName = "preferred_username";

            /// <summary>
            /// Old TenantId claim: "http://schemas.microsoft.com/identity/claims/tenantid".
            /// </summary>
            public const string TenantIdClaimKeyOfficial = "http://schemas.microsoft.com/identity/claims/tenantid";

            /// <summary>
            /// New Tenant Id claim: "tid".
            /// </summary>
            public const string TenantIdClaimKey = "tid";

            /// <summary>
            /// ClientInfo claim: "client_info".
            /// </summary>
            public const string ClientInfo = "client_info";

            /// <summary>
            /// UniqueObjectIdentifier: "uid".
            /// Home Object Id.
            /// </summary>
            public const string UniqueObjectIdentifier = "uid";

            /// <summary>
            /// UniqueTenantIdentifier: "utid".
            /// Home Tenant Id.
            /// </summary>
            public const string UniqueTenantIdentifier = "utid";

            /// <summary>
            /// Older scope claim: "http://schemas.microsoft.com/identity/claims/scope".
            /// </summary>
            public const string Scope = "http://schemas.microsoft.com/identity/claims/scope";

            /// <summary>
            /// Newer scope claim: "scp".
            /// </summary>
            public const string Scp = "scp";

            /// <summary>
            /// New Roles claim = "roles".
            /// </summary>
            public const string Roles = "roles";

            /// <summary>
            /// Old Role claim: "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".
            /// </summary>
            public const string Role = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

            /// <summary>
            /// Subject claim: "sub".
            /// </summary>
            public const string Sub = "sub";

            /// <summary>
            /// Acr claim: "acr".
            /// </summary>
            public const string Acr = "acr";

            /// <summary>
            /// UserFlow claim: "http://schemas.microsoft.com/claims/authnclassreference".
            /// </summary>
            public const string UserFlow = "http://schemas.microsoft.com/claims/authnclassreference";

            /// <summary>
            /// Tfp claim: "tfp".
            /// </summary>
            public const string Tfp = "tfp";

            /// <summary>
            /// Name Identifier ID claim: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier".
            /// </summary>
            public const string NameIdentifierId = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

            /// <summary>
            /// Version claim: "ver".
            /// </summary>
            public const string Version = "ver";

            /// <summary>
            /// AppIdACR claim: "appidacr".
            /// </summary>
            public const string AppIdACR = "appidacr";

            /// <summary>
            /// Application Id claim: "appid".
            /// </summary>
            public const string ApplicationId = "appid";
        }
    }
}
