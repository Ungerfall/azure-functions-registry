using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EricLippertBlogRoulette
{
    public static class Function1
    {
        private static Random rng = new Random();

        [FunctionName("GetEricLippertBlogArticle")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Blob("functions-data/visited", FileAccess.Read)] TextReader inputBlob,
            [Blob("functions-data/visited", FileAccess.Write)] TextWriter outputBlob,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            using var webClient = new WebClient();
            string linksJson = await webClient.DownloadStringTaskAsync(
                "https://learning-fapp-4928.azurewebsites.net/api/GetEricLippertBlogArticles?code=1gLGWOODXcmJHVs6PLQSBcSyM0dHL/JPt1NTwgUJTvLHTurY61yUbg==");
            var visited = new List<string>();
            if (inputBlob != null)
            {
                string visitedLink;
                while ((visitedLink = inputBlob.ReadLine()) != null)
                {
                    visited.Add(visitedLink);
                }
            }

            var links = JsonConvert.DeserializeObject<string[]>(linksJson)
                .Except(visited)
                .ToArray();
            var randomLink = links[rng.Next(links.Length)];

            visited.Add(randomLink);
            foreach (var link in visited)
            {
                outputBlob.WriteLine(link);
            }

            return new RedirectResult(randomLink);
        }
    }
}
