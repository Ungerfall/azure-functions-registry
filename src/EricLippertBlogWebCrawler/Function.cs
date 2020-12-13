using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EricLippertBlogWebCrawler
{
	public static class Function
	{
		[FunctionName("GetEricLippertBlogArticles")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
			[Blob("eric-lippert-articles/eric-lippert-articles.txt", FileAccess.Read)] Stream blob,
			ILogger log)
		{
			log.LogInformation("C# HTTP trigger function processed a request.");
			using StreamReader sr = new StreamReader(blob, Encoding.UTF8);
			var links = new List<string>();

			string line;
			while ((line = sr.ReadLine()) != null)
			{
				links.Add(line);
			}

			var json = JsonConvert.SerializeObject(links);

			return new OkObjectResult(json);
		}
	}
}
