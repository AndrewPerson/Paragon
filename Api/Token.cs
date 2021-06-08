using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Paragon.Shared;

namespace Paragon.Oauth2
{
    public class Token : TableEntity
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiry { get; set; }
        public bool Expired => DateTime.Now > Expiry;
        public DateTime Termination { get; set; }
        public bool Terminated => DateTime.Now > Termination;

        public Token() => PartitionKey = "Tokens";

        public static async Task<Token> FromAuthCode(string authCode)
        {
            Token result = new Token();

            var token = await SharedData.HttpClient.PostAsync(SharedData.Url + "/token", new FormUrlEncodedContent
            (
                new Dictionary<string, string>
                {
                    {"code", authCode},
                    {"grant_type", "authorization_code"},
                    {"client_id", SharedData.Id},
                    {"client_secret", SharedData.Secret},
                    {"redirect_uri", SharedData.Url + "/redirect"}
                }
            ));

            var root = JsonDocument.Parse(token.Content.ReadAsStringAsync().Result).RootElement;

            result.AccessToken = root.GetProperty("access_token").GetString();
            result.RefreshToken = root.GetProperty("refresh_token").GetString();
            result.Expiry = DateTime.Now.AddSeconds(root.GetProperty("expires_in").GetInt32());
            result.Termination = DateTime.Now.AddDays(root.GetProperty("").GetInt32());

            return result;
        }

        public static async Task<Token> GetToken(string credentials)
        {
            TableOperation operation = TableOperation.Retrieve<Token>("Tokens", credentials);
            CloudTable table = new CloudTable(SharedData.TokenTableUri, SharedData.TokenTableCredentials);
            var result = await table.ExecuteAsync(operation);

            if (result.HttpStatusCode == 200) return (Token) result.Result;
            return null;
        }

        public static async Task SetToken(string credentials, Token token)
        {
            token.RowKey = credentials;
            TableOperation operation = TableOperation.InsertOrReplace(token);
            CloudTable table = new CloudTable(SharedData.TokenTableUri, SharedData.TokenTableCredentials);
            await table.ExecuteAsync(operation);
        }

        public async Task Refresh()
        {
            var token = await SharedData.HttpClient.PostAsync(SharedData.Url + "/token", new FormUrlEncodedContent
            (
                new Dictionary<string, string>
                {
                    {"refresh_token", RefreshToken},
                    {"grant_type", "refresh_token"},
                    {"client_id", SharedData.Id},
                    {"client_secret", SharedData.Secret}
                }
            ));

            var root = JsonDocument.Parse(await token.Content.ReadAsStringAsync()).RootElement;

            AccessToken = root.GetProperty("access_token").GetString();
            RefreshToken = root.GetProperty("refresh_token").GetString();
            Expiry = DateTime.Now.AddSeconds(root.GetProperty("expires_in").GetInt32());
            Termination = DateTime.Now.AddDays(root.GetProperty("").GetInt32());
        }

        public void Sign(ref HttpRequestMessage message) => message.Headers.Add("Authorization", "Bearer " + AccessToken);
    }
}