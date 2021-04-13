using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.AuthServer
{
    public static class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>()
            {
                new ApiResource("resource_api1"){
                    Scopes ={ "api1.read", "api1.write", "api1.update" },
                    ApiSecrets = new[]{new Secret("secretapi1".Sha256()) }
                },
                new ApiResource("resource_api2"){
                    Scopes = { "api2.read", "api2.write", "api2.update" },
                    ApiSecrets = new[]{new Secret("secretapi1".Sha256()) }
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>()
            {
                new ApiScope("api1.read","API 1 için okuma iznidir."),
                new ApiScope("api1.write","API 1 için yazma iznidir."),
                new ApiScope("api1.update","API 1 için güncelleme iznidir."),
                new ApiScope("api2.read","API 2 için okuma iznidir."),
                new ApiScope("api2.write","API 2 için yazma iznidir."),
                new ApiScope("api2.update","API 2 için güncelleme iznidir."),
                new ApiScope("api3.read","API 3 için okuma iznidir."),
                new ApiScope("api3.write","API 3 için yazma iznidir."),
                new ApiScope("api3.update","API 3 için güncelleme iznidir.")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client()
                {
                    ClientId = "Client1",
                    ClientName = "Client 1 App",
                    ClientSecrets = new[]{new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"api1.read"}
                },
                new Client()
                {
                    ClientId = "Client2",
                    ClientName = "Client 2 App",
                    ClientSecrets = new[]{new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"api1.read","api1.update","api2.write","api2.update"}
                },
                 new Client()
                {
                    ClientId = "Client1-Mvc",
                    RequirePkce = false,
                    ClientName = "Client 1 App",
                    ClientSecrets = new[]{new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RedirectUris = new List<string>{"https://localhost:5003/signin-oidc" },
                    PostLogoutRedirectUris = new List<string>{ "https://localhost:5003/signout-callback-oidc" },
                    AllowedScopes = {IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, "api1.read",IdentityServerConstants.StandardScopes.OfflineAccess, "Roles"},
                    AccessTokenLifetime  = 2*60*60,
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    AbsoluteRefreshTokenLifetime = (DateTime.Now.AddDays(60)-DateTime.Now).Seconds,
                    RequireConsent = true
                },
                 new Client()
                {
                    ClientId = "Client2-Mvc",
                    RequirePkce = false,
                    ClientName = "Client 2 App",
                    ClientSecrets = new[]{new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RedirectUris = new List<string>{"https://localhost:5005/signin-oidc" },
                    PostLogoutRedirectUris = new List<string>{ "https://localhost:5005/signout-callback-oidc" },
                    AllowedScopes = {IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, "api1.read",IdentityServerConstants.StandardScopes.OfflineAccess, "Roles"},
                    AccessTokenLifetime  = 2*60*60,
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    AbsoluteRefreshTokenLifetime = (DateTime.Now.AddDays(60)-DateTime.Now).Seconds,
                    RequireConsent = true
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>()
            {
                new IdentityResources.OpenId(), //subId
                new IdentityResources.Profile(),
                new IdentityResource(){Name = "Roles", DisplayName = "Roles", Description = "Kullanıcı Rolleri", UserClaims = new[]{"role"}}
             };
        }
        public static IEnumerable<TestUser> GetUsers()
        {
            return new List<TestUser>()
            {
                new TestUser
                {
                    SubjectId = "1", Username ="mbarisugur",Password= "password", Claims = new List<Claim>()
                    {
                        new Claim("given_name","Barış"),
                        new Claim("family_name", "Uğur"),
                        new Claim("role","user")
                    }
               },
                new TestUser
                {
                    SubjectId = "2", Username ="onurdogan",Password= "password", Claims = new List<Claim>()
                    {
                        new Claim("given_name","Onur"),
                        new Claim("family_name", "Dogan"),
                        new Claim("role","admin")
                    }
               }
            };
        }

    }
}
