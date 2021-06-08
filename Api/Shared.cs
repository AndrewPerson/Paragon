using System;
using System.Net.Http;
using System.Security.Cryptography;
using Microsoft.Azure.Cosmos.Table;

namespace Paragon.Shared
{
    public static class SharedData
    {
        public static readonly string Id = Environment.GetEnvironmentVariable("ID");
        public static readonly string Secret = Environment.GetEnvironmentVariable("SECRET");
        public static readonly string Url =
            (Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") == "Development" ?
            "http://" : "https://") +
            Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
        public static readonly HttpClient HttpClient = new HttpClient();
        public static readonly StorageCredentials TokenTableCredentials = new StorageCredentials("paragonstorage", "p8bci4riF7klvfyfbBfGhw4OGZ33pWJj/T183eGeAdmyQhUSxUWVcnLeMncQoh8pWGfhqStxNr7CIoPqf3GOnQ==");
        public static readonly Uri TokenTableUri = new Uri("https://paragonstorage.table.core.windows.net/tokens");
        /*
        private static readonly byte[] Key = new byte[] { 84, 194, 68, 50, 5, 159, 187, 118, 232, 5, 105, 229, 10, 67, 199, 8, 
                                                          149, 45, 169, 109, 37, 23, 47, 162, 31, 174, 129, 125, 116, 174, 102, 203, 
                                                          164, 220, 150, 149, 234, 52, 224, 250, 211, 75, 171, 134, 121, 226, 200, 70, 
                                                          13, 172, 194, 232, 17, 106, 30, 174, 21, 196, 168, 145, 219, 54, 47, 91, 
                                                          54, 117, 223, 184, 117, 250, 192, 120, 245, 228, 120, 35, 35, 250, 118, 252,
                                                          90, 130, 232, 77, 54, 122, 167, 73, 161, 120, 44, 91, 23, 237, 240, 228, 
                                                          125, 74, 55, 182, 210, 28, 38, 181, 239, 195, 219, 115, 232, 233, 206, 222,
                                                          203, 224, 175, 82, 104, 40, 133, 2, 153, 226, 162, 20, 131, 131, 20, 88 };
        public static readonly HMACSHA512 Hash = new HMACSHA512(Key);
        */
    }
}