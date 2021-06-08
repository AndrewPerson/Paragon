using System.Threading.Tasks;
using System.Text.Encodings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paragon.Oauth2;
using Paragon.Shared;

namespace Paragon.Functions
{
    public static class OAuth2Auth
    {
        [FunctionName("auth")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, 
            ILogger log)
        {
            if (!req.HasFormContentType) return new BadRequestObjectResult("Body must have form content type");

            var body = await req.ReadFormAsync();            

            if (body.ContainsKey("code"))
            {
                Token token = await Token.FromAuthCode(body["code"]);
                await Token.SetToken(body["code"], token);
                return new OkResult();
            }
            else return new BadRequestObjectResult("Body does not contain a code");
        }
    }
}
