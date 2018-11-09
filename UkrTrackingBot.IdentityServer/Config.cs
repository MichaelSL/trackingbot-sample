using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UkrTrackingBot.IdentityServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("npapi", "Nova Poshta API"),
                new ApiResource("delapi", "Delivery API")
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(MvcClientConfigValues mvcClientConfig)
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = "telegrambotclient",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("telegrambotsecret".Sha256())    //800397e847831480ea06cd25d345de22c1d19aa5802fdcc2a31b682dda3bf932
                    },
                    AllowedScopes = { "npapi", "delapi" }
                },

                // OpenID Connect hybrid flow and client credentials client (MVC)
                new Client
                {
                    ClientId = "marketingmvcclient",
                    ClientName = "Web Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    RequireConsent = false,

                    ClientSecrets =
                    {
                        new Secret("marketingmvcsecret".Sha256())   //fa8ec072f28a19632916c488b9c27e01835a22a6630fa1cc4535ce2d88af3cc7
                    },

                    RedirectUris = { mvcClientConfig.RedirectUri },
                    PostLogoutRedirectUris = { mvcClientConfig.PostLogoutRedirectUri },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "npapi"
                    },
                    AllowOfflineAccess = true
                }
            };
        }
    }

    public class MvcClientConfigValues
    {
        public string RedirectUri { get; set; }
        public string PostLogoutRedirectUri { get; set; }
    }
}
