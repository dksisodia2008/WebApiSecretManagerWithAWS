using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http.Json;
using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;



namespace WebApiSecretManagerWithAWS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AwsSecretManagerController : ControllerBase
    {
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {

            var result = await IsLogin(username, password);
            if (result.IsSuccess !=true)
            {
                return BadRequest("Login failed.");
            }

            return Ok("Successfully login");
        }

        private async Task<LogicResponse> IsLogin(string username, string password)
        {
            var secret = await GetSecret();
            var jsonObject = JObject.Parse(secret);

            string _userName = (string)jsonObject["username"];
            string _password = (string)jsonObject["password"];


            if (username != _userName && password != _password)
            {
                return new LogicResponse()
                {
                    IsSuccess = false,
                    Message = "Username and password are not correct"
                };
            }
            return new LogicResponse()
            {
                IsSuccess = true,
                Message = "Login Successfully"
            };
        }
        static async  Task<string> GetSecret()
        {
            string secretName = "dev/myawssecret/login";
            string region = "ap-south-1";

            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

            GetSecretValueRequest request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT", // VersionStage defaults to AWSCURRENT if unspecified.
            };

            GetSecretValueResponse response;

            try
            {
                response = await client.GetSecretValueAsync(request);
            }
            catch (Exception e)
            {
                throw e;
            }

            return response.SecretString;
        }
    }
}
