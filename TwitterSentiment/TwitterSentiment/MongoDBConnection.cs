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
        private static IMongoCollection<Tweet> collection2;


        private MongoDBConnection()
        {
            client = new MongoClient("");
            database = client.GetDatabase("Tweets");
            collection = database.GetCollection<BsonDocument>("Sentiment");
            collection2 = database.GetCollection<Tweet>("Sentiment");

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
                var filter = Builders<BsonDocument>.Filter.Eq("tweetID", tweet.tweetId);
                var tweetDocument = collection.Find(filter).FirstOrDefault();
                if (tweetDocument==null)
                {
                    var document = new BsonDocument {
                        { "tweetID", $"{tweet.tweetId}" },
                        { "TweetText",$"{tweet.TweetText}" },
                        { "SentimentScore", tweet.SentimentScore }
                    };
                    collection.InsertOne(document);

                }

            }

            

        }
    }
}
