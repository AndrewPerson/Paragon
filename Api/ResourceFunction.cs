using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Paragon.Shared;
using Paragon.Oauth2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Paragon.Functions
{
    public static class Resource
    {
        public static readonly Dictionary<string, string> Resources = new Dictionary<string, string>
        {
            {"daytimetable", "timetable/daytimetable.json"},
            {"timetable", "timetable/timetable.json"},
            {"userinfo", "details/userinfo.json"},
            {"participation", "details/participation.json"}
        };

        [FunctionName("resource")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            ILogger log)
        {
            if (!req.Query.TryGetValue("resource", out var resourceString))
                return new BadRequestObjectResult("Query must contain resource");

            if (!Resources.TryGetValue(resourceString, out var resource))
                return new BadRequestObjectResult("Resource specified is not valid");

            if (!req.Query.TryGetValue("code", out var code))
                return new BadRequestObjectResult("Query must contain code");
            
            var token = await Token.GetToken(code);
            if (token == null) return new BadRequestObjectResult("There is no data associated with the specified code");

            var message = new HttpRequestMessage(HttpMethod.Get, "https://student.sbhs.net.au/api/" + resource);
            token.Sign(ref message);

            var response = await SharedData.HttpClient.SendAsync(message);
            return new OkObjectResult(await response.Content.ReadAsStringAsync());
        }
    }
}