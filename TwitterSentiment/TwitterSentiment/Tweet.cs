using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TwitterSentiment
{
    public class Tweet {
        public Tweet(string id, string txt, string score) => (this.Id, this.Text, this.SentimentScore) = (id, txt, score);

        public string Id;
        public string Text;
        public string SentimentScore;

        public static List<Tweet> ParseJsonToTweetObjects(string input)
        {
            var arrayOfTweets = JsonConvert.DeserializeObject<List<string>>(input);

            List<Tweet> tweets = new List<Tweet>();
            foreach (var tweet in arrayOfTweets)
            {
                var elements = tweet.Split("|||");
                tweets.Add(new Tweet(elements[0], elements[1], elements[2]));
            }

            return tweets;
        }

        public override string ToString()
        {
            return $"Score: {SentimentScore}";
        }
    }
}
