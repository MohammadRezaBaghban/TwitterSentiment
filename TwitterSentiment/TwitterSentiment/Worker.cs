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
using MongoDB.Driver;
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
                try
                {
                    //A REST GET Endpoint created and exposed using Azure Logic App 
                    HttpResponseMessage response = client.GetAsync("https://prod-104.westeurope.logic.azure.com/workflows/0fe02adb350546fd8b90d52d0eb5ad60/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=osX5anjLkff-II9SFa8Q9O-S7tucOfbfferPCZOcZDw").Result;
                    Console.WriteLine("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                    Console.WriteLine("Response Message Header \n\n" + response.Content.Headers + "\n");

                    var JsonString = await response.Content.ReadAsStringAsync();
                    var tweets = Tweet.ParseJsonToTweetObjects(JsonString);
                    var dbConnection = MongoDBConnection.GetConnectionObject();

                    if (response.IsSuccessStatusCode)
                    {
                        dbConnection.InsertToDatabase(tweets);
                        _logger.LogInformation($"Tweets has been inserted");
                    }
                    else
                    {
                        _logger.LogError($"Something Went Wrong");
                    }

                    await Task.Delay(3 * 60 * 1000, stoppingToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }



    }
}
