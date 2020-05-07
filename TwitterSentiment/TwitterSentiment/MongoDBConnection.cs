using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TwitterSentiment
{
    
    public class MongoDBConnection
    {
        private static MongoDBConnection DbConnection;
        private MongoClient client;
        private IMongoDatabase database;
        private static IMongoCollection<BsonDocument> collection;

        private MongoDBConnection()
        {
            client = new MongoClient("");
            database = client.GetDatabase("Tweets");
            collection = database.GetCollection<BsonDocument>("Sentiment");
        }

        public static MongoDBConnection GetConnectionObject()
        {
            if (DbConnection == null)
            {
                DbConnection = new MongoDBConnection();
                return DbConnection;
            }
            else
            {
                return DbConnection;
            }
        }

        public async void InsertToDatabase(ICollection<Tweet> tweets)
        {
            

            foreach (var tweet in tweets)
            {
                var document = new BsonDocument {
                    { "tweetID", $"{tweet.Id}" },
                    { "TweetText",$"{tweet.Text}" },
                    { "SentimentScore", tweet.SentimentScore }
                };

                await collection.InsertOneAsync(document);
            }

            

        }
    }
}
