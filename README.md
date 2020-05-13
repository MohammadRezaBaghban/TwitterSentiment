# Twitter Sentiment
A .NET Core 3.0 Worker Service to use Azure Cognitive Services to evaluate sentiment of tweets

# Azure Logic App and Cognitive Service
The application use A RESTful API that was exposed using a Microsoft Azure Logic App that checks 30 tweets of startup hashtag and try to analysis the text sentiment and return the result as JSON array with each tweet id,text and sentiment score.

# .NET Core Worker Service
The worker service try to communicate with abovementioned endpoint every three minuets and parse the JSON response into Tweet Objects and then Interact with MongoDB Document Based Database to Insert records that were not being there.
