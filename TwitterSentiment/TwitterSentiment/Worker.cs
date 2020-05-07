using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TwitterSentiment
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient client;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            
            client = new HttpClient();
            return base.StartAsync(cancellationToken);
        }


        public override Task StopAsync(CancellationToken cancellationToken)
        {
            client.Dispose();
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                HttpResponseMessage response = client.GetAsync("https://prod-104.westeurope.logic.azure.com/workflows/0fe02adb350546fd8b90d52d0eb5ad60/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=osX5anjLkff-II9SFa8Q9O-S7tucOfbfferPCZOcZDw").Result;
                Console.WriteLine("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                Console.WriteLine("Response Message Header \n\n" + response.Content.Headers + "\n");
                
                var customerJsonString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var arrayOfTweets = JsonConvert.DeserializeObject<List<string>>(customerJsonString);
                    List<Tweet> tweets = new List<Tweet>();
                    foreach (var tweet in arrayOfTweets)
                    {
                        var elements = tweet.Split("|||");
                        tweets.Add(new Tweet(elements[0], elements[1], elements[2]));
                    }
                    _logger.LogInformation($"The website is up. Status code {response.StatusCode}");
                    Console.WriteLine(customerJsonString);
                }
                else
                {
                    _logger.LogError($"The website is down. Status {response.StatusCode}");
                }
               
            }
        }

        public class Tweet
        {
            public Tweet(string id, string txt, string score) => (this.Id, this.Text, this.SentimentScore) = (id,txt,score);

            public string Id;
            public string Text;
            public string SentimentScore;

            public override string ToString()
            {
                return $"Score: {SentimentScore}";
            }
        }
    }
}
