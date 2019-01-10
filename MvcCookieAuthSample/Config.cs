using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace MvcCookieAuthSample
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>{
                new ApiResource("api1","My API1")
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>{
                new Client{
                    ClientId="mvc",
                    ClientName="Mvc Client",
                    ClientUri="http://localhost:5001",
                    LogoUri="https://temilaj.gallerycdn.vsassets.io/extensions/temilaj/asp-net-core-vs-code-extension-pack/1.0.3/1534374071581/Microsoft.VisualStudio.Services.Icons.Default",
                    AllowRememberConsent=true,
                    
                    ClientSecrets={
                        new Secret("secret".Sha256())
                    },
                    AllowedGrantTypes=GrantTypes.Implicit,

                    RedirectUris={ "http://localhost:5001/signin-oidc"},
                    PostLogoutRedirectUris={ "http://localhost:5001/signout-callback-oidc"},

                                     //AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes={
                         IdentityServerConstants.StandardScopes.OpenId,
                         IdentityServerConstants.StandardScopes.Profile
                    },

                    RequireConsent=true
                }
            };
        }
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>{
                new TestUser{
                    SubjectId="1",
                    Username="hhz",
                    Password="123456",
                    //Claims = new List<Claim>()
                    //{
                    //    new Claim("name","hhz"),
                    //    new Claim("website","www.cnblogs.com")
                    //}
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>{
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }
    }
}