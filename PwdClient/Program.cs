﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace PwdClient
{
    class Program
    {
        static void Main(string[] args)=>MainAsync().GetAwaiter().GetResult();
        static async Task MainAsync()
        {
            //1.http://localhost:5000/.well-known/openid-configuration
            var disc=await DiscoveryClient.GetAsync("http://localhost:5000");
            if(disc.IsError)
                System.Console.WriteLine(disc.Error);
            
            var tokenClient=new TokenClient(disc.TokenEndpoint,"pwd_client","secret");
            //2.http://localhost:5000/connect/token
            var tokenResponse=await tokenClient.RequestResourceOwnerPasswordAsync("tom","123456","api");
            if(tokenResponse.IsError)
                System.Console.WriteLine(tokenResponse.Error);
            System.Console.WriteLine(tokenResponse.Json);

            var client=new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            //3.http://localhost:5001/api/Values
            var result= await client.GetAsync("http://localhost:5001/api/Values");
        
            System.Console.WriteLine(result.Content.ReadAsStringAsync().Result);
            Console.ReadKey();
        }
    }
}
