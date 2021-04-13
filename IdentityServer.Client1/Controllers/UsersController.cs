using IdentityModel.Client;
using IdentityServer.Client1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityServer.Client1.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IConfiguration _configuration;

        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            HttpClient httpClient = new HttpClient();
            List<User> users = new List<User>();
            //var discovery = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");
            //if (discovery.IsError)
            //{
            //    Console.WriteLine("Discovery hatası alındı.");
            //}
            //ClientCredentialsTokenRequest clientCredentialsToken = new ClientCredentialsTokenRequest();

            //clientCredentialsToken.ClientId = _configuration["Client:ClientId"];
            //clientCredentialsToken.ClientSecret = _configuration["Client:ClientSecret"];
            //clientCredentialsToken.Address = discovery.TokenEndpoint;

            //var token = await httpClient.RequestClientCredentialsTokenAsync(clientCredentialsToken);

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            httpClient.SetBearerToken(accessToken);

            var response = await httpClient.GetAsync("https://localhost:5007/api/users/getusers");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                users = JsonConvert.DeserializeObject<List<User>>(content);
            }
            else
            {
                Console.WriteLine("response hatası");
            }

            return View(users);
        }

        public IActionResult UserInfo()
        {
            return View();
        }


        public async Task LogOut()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }

        public async Task<IActionResult> GetRefreshToken()
        {
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            HttpClient httpClient = new HttpClient();
            var discovery = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");

            RefreshTokenRequest refreshTokenRequest = new RefreshTokenRequest();
            refreshTokenRequest.ClientId = _configuration["Client1Mvc:ClientId"];
            refreshTokenRequest.ClientSecret = _configuration["Client1Mvc:ClientSecret"];
            refreshTokenRequest.RefreshToken = refreshToken;
            refreshTokenRequest.Address = discovery.TokenEndpoint;
            var token = await httpClient.RequestRefreshTokenAsync(refreshTokenRequest);
            if (token.IsError)
            {
                Console.WriteLine("Token hatası !");
            }
            var tokens = new List<AuthenticationToken>()
            {
                new AuthenticationToken{Name = OpenIdConnectParameterNames.IdToken, Value = token.IdentityToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.AccessToken, Value = token.AccessToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.RefreshToken, Value = token.RefreshToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.ExpiresIn, Value = DateTime.UtcNow.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}
            };

            var authenticationResult = await HttpContext.AuthenticateAsync();
            var properties = authenticationResult.Properties;


            properties.StoreTokens(tokens);

            await HttpContext.SignInAsync("Cookies", authenticationResult.Principal, properties);

            return RedirectToAction("Index");
        }

        [Authorize(Roles ="admin")]
        public IActionResult AdminAction()
        {
            // sdfasflkdfjasdjfklajsdkfljaskf

            return View();
        }


        [Authorize(Roles = "user")]
        public IActionResult UserAction()
        {
            return View();
        }

    }
}
