using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Duende.Identity.Server;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };

    /*public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("api1", "My API")
        };*/

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "razorclient",
                ClientName = "Razor Web App",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,

                RedirectUris = { "http://localhost:5273/signin-oidc" },
                PostLogoutRedirectUris = { "http://localhost:5273/signout-callback-oidc" },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email
                },
                AllowAccessTokensViaBrowser = true,
                RequirePkce = true,
                AllowPlainTextPkce = true
            }
        };
}