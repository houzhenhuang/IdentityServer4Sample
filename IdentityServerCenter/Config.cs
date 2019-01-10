using System.Collections;
using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServerCenter
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetResources()
        {
            return new List<ApiResource>{
                new ApiResource("api","My Api")
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>{
                new Client{
                    ClientId="client",
                    AllowedGrantTypes=GrantTypes.ClientCredentials,

                    ClientSecrets={
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes={"api"}
                },
                new Client{
                    ClientId="pwd_client",
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,

                    ClientSecrets={
                        new Secret("secret".Sha256())
                    },
                    RequireClientSecret=false,//获取token的时候可以不用带密码过来，一般用于信任程度很高的第三方(client)
                    AllowedScopes={"api"}
                }
            };
        }
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>{
                new TestUser{
                    SubjectId="1001",
                    Username="tom",
                    Password="123456"
                }
            };
        }
    }
}