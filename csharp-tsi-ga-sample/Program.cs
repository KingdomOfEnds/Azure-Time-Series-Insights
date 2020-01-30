using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TimeSeriesInsightsQuerySample
{
    class Program
    {
        // Azure Time Series Insights environment configuration
        internal static string EnvironmentFqdn = "10000000-0000-0000-0000-100000000108.env.timeseries.azure.com";

        // Azure Active Directory application configuration
        internal static string AadClientApplicationId = "#PLACEHOLDER#";
        internal static string AadClientSecret = "#PLACEHOLDER#";
        internal static string[] AadScopes = new string[] { "https://api.timeseries.azure.com/.default" };
        internal static string AadRedirectUri = "http://localhost:8080/";
        internal static string AadTenantName = "#PLACEHOLDER#";
        internal static string AadAuthenticationAuthority = "https://login.microsoftonline.com/" + AadTenantName + ".onmicrosoft.com/oauth2/authorize?resource=https://api.timeseries.azure.com/";

        private static async Task<string> AcquireAccessTokenAsync()
        {
            if (AadClientApplicationId == "#PLACEHOLDER#" || AadClientSecret == "#PLACEHOLDER#" || AadScopes.Length == 0 || AadRedirectUri == "#PLACEHOLDER#" || AadTenantName.StartsWith("#PLACEHOLDER#"))
            {
                throw new Exception($"Use the link {"https://docs.microsoft.com/azure/time-series-insights/time-series-insights-get-started"} to update the values of 'AadClientApplicationId', 'AadScopes', 'AadRedirectUri', and 'AadAuthenticationAuthority'.");
            }

            /**
             * MSAL.NET configuration. Review the product documentation for more information about MSAL.NET authentication options.
             * 
             * https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/
             */
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
                .Create(AadClientApplicationId)
                .WithClientSecret(AadClientSecret)
                .WithRedirectUri(AadRedirectUri)
                .WithAuthority(AadAuthenticationAuthority)
                .Build();

            AuthenticationResult result = await app
                .AcquireTokenForClient(AadScopes)
                .ExecuteAsync();

            Console.WriteLine("MSAL Authentication Token Acquired: {0}", result.AccessToken);
            Console.WriteLine("");
            return result.AccessToken;
        }

        // System.Net.HttpClient helper to wrap HTTP GET requests
        private static async Task<HttpResponseMessage> AsyncHttpGetRequestHelper(HttpClient httpClient, Uri uri)
        {
            Console.WriteLine("Making HTTP GET to URI: {0}", uri);
            Console.WriteLine("");
            HttpResponseMessage response = await httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jsonStringTransferObject = JsonConvert.DeserializeObject<object>(jsonString);
                Console.WriteLine("HTTP JSON Response Body: {0}", jsonStringTransferObject);
                Console.WriteLine("");
                return response;
            }
            return null;
        }

        // System.Net.HttpClient helper to wrap HTTP POST requests
        private static async Task<HttpResponseMessage> AsyncHttpPostRequestHelper(HttpClient httpClient, Uri uri, string input)
        {
            Console.WriteLine("HTTP JSON Request Body: {0}", input);
            Console.WriteLine("");
            HttpContent requestBody = new StringContent(input, Encoding.UTF8, "application/json");

            Console.WriteLine("Making HTTP POST to URI: {0}", uri);
            Console.WriteLine("");
            HttpResponseMessage response = await httpClient.PostAsync(uri, requestBody);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jsonStringTransferObject = JsonConvert.DeserializeObject<object>(jsonString);
                Console.WriteLine("HTTP JSON Response Body: {0}", jsonStringTransferObject);
                Console.WriteLine("");
                return response;
            }
            return null;
        }

        // System.Net.HttpClient helper to wrap WSS messages
        private static async Task<List<JToken>> AsyncWssHelper(ClientWebSocket socket, Uri uri, string input)
        {
            // WSS connect
            await socket.ConnectAsync(uri, CancellationToken.None);
            byte[] inputPayloadBytes = Encoding.UTF8.GetBytes(input);

            // WSS send message
            await socket.SendAsync(
                new ArraySegment<byte>(inputPayloadBytes),
                WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None);

            // WSS handle response messages
            List<JToken> responseMessagesContent = new List<JToken>();

            using (socket)
            {
                while (true)
                {   
                    // Read response message as it arrives
                    string message;
                    using (var ms = new MemoryStream())
                    {
                        const int bufferSize = 16 * 1024;
                        var temporaryBuffer = new byte[bufferSize];
                        while (true)
                        {
                            WebSocketReceiveResult response = await socket.ReceiveAsync(
                                new ArraySegment<byte>(temporaryBuffer),
                                CancellationToken.None);

                            ms.Write(temporaryBuffer, 0, response.Count);
                            
                            if (response.EndOfMessage)
                            {
                                break;
                            }
                        }

                        ms.Position = 0;
                        using (var sr = new StreamReader(ms))
                        {
                            message = sr.ReadToEnd();
                        }
                    }

                    JObject messageObj = JsonConvert.DeserializeObject<JObject>(message);

                    // WSS error handling
                    if (messageObj["error"] != null)
                    {
                        break;
                    }

                    responseMessagesContent.Add(messageObj["content"]);

                    // WSS check percent completed - stop at 100% completed
                    if (messageObj["percentCompleted"] != null && Math.Abs((double)messageObj["percentCompleted"] - 100d) < 0.01)
                    {
                        break;
                    }
                }

                // WSS close open connections once completed
                if (socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "CompletedByClient",
                        CancellationToken.None);
                }
            }
            return responseMessagesContent;
        }

        // Main app logic
        private static async Task TsiGaSample() {

            if (EnvironmentFqdn.StartsWith("#PLACEHOLDER#"))
            {
                throw new Exception($"Use the link {"https://docs.microsoft.com/azure/time-series-insights/time-series-insights-authentication-and-authorization"} to update the values of 'EnvironmentFqdn' and 'EnvironmentReferenceDataSetName'.");
            }

            Console.WriteLine("Beginning demo...");
            Console.WriteLine("");

            string accessToken = await AcquireAccessTokenAsync();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            httpClient.DefaultRequestHeaders.Add("x-ms-client-application-name", "TimeSeriesInsightsQuerySample");

            var socket = new ClientWebSocket();

            /**
             * GET Environments API
             * https://docs.microsoft.com/rest/api/time-series-insights/ga-query-api#get-environments-api
             */

            Uri getEnvironmentsUri = new UriBuilder("https", "api.timeseries.azure.com")
            {
                Path = "/environments",
                Query = "api-version=2016-12-12"
            }.Uri;
            var getEnvironmentsResponse = await AsyncHttpGetRequestHelper(httpClient, getEnvironmentsUri);

            /**
             * GET Environment Availability API
             * https://docs.microsoft.com/rest/api/time-series-insights/ga-query-api#get-environment-availability-api
             */
            
            Uri getAvailabilityUri = new UriBuilder("https", EnvironmentFqdn)
            {
                Path = "/availability",
                Query = "api-version=2016-12-12"
            }.Uri;
            var getAvailabilityResponse = await AsyncHttpGetRequestHelper(httpClient, getAvailabilityUri);
            /** 
             * var range = getAvailabilityResponse["range"];
             * 
             *     
             * GET Environment Metadata API
             * https://docs.microsoft.com/rest/api/time-series-insights/ga-query-api#get-environment-metadata-api
             

            string postMetadataInput = @"
                {
                    ""searchSpan"": {
                        ""from"": {     
                            ""dateTime"":""" + range["from"].Value<DateTime>() + @"""},
                        ""to"": {
                            ""dateTime"":""" + range["to"].Value<DateTime>() + @"""}
                     }
                }";

            Uri postMetadataUri = new UriBuilder("https", EnvironmentFqdn)
            {
                Path = "/metadata",
                Query = "api-version=2016-12-12"
            }.Uri;

            var postMetadataResponse = await AsyncHttpPostRequestHelper(httpClient, postMetadataUri, postMetadataInput);

            
             * GET Environment Events API
             * https://docs.microsoft.com/rest/api/time-series-insights/ga-query-api#get-environment-events-api
             

            string getEventsInput = @"
                {
                    ""searchSpan"": {
                        ""from"": {     
                            ""dateTime"":""" + range["from"].Value<DateTime>() + @"""},
                        ""to"": {
                            ""dateTime"":""" + range["to"].Value<DateTime>() + @"""}
                    },
                    ""take"": 10
                }";

            Uri getEventsUri = new UriBuilder("https", EnvironmentFqdn)
            {
                Path = "/events",
                Query = "api-version=2016-12-12"
            }.Uri;

            var getEventsResponse = await AsyncHttpPostRequestHelper(httpClient, getEventsUri, getEventsInput);

            
             * WSS Get Environment Events Streamed API
             * https://docs.microsoft.com/rest/api/time-series-insights/ga-query-api#get-environment-events-streamed-api
             

            string wssEventsInput = @"
                {
                    ""headers"": {
                        ""x-ms-client-application-name"": ""TimeSeriesInsightsQuerySample"",
                        ""Authorization"": ""Bearer """ + accessToken + @"
                    },
                    ""content"":" + getEventsInput +
                "}";

            Uri wssEventsUri = new UriBuilder("wss", EnvironmentFqdn)
            {
                Path = "/events",
                Query = "api-version=2016-12-12"
            }.Uri;

            var wssEventsResponse = await AsyncWssHelper(socket, wssEventsUri, wssEventsInput);

            
             * WSS Get Environment Aggregates Streamed API
             * https://docs.microsoft.com/rest/api/time-series-insights/ga-query-api#get-environment-aggregates-streamed-api
             

            string wssAggregatesInput = @"
                {
                    ""headers"": {
                        ""x-ms-client-application-name"": ""TimeSeriesInsightsQuerySample"",
                        ""Authorization"": ""Bearer """ + accessToken + @"
                    },
                    ""content"": { 
                        ""predicate"":{  
                            ""predicateString"": """"
                        },
                        ""searchSpan"":{  
                            ""from"": {     
                                ""dateTime"":""" + range["from"].Value<DateTime>() + @"""},
                            ""to"": {
                                ""dateTime"":""" + range["to"].Value<DateTime>() + @"""}
                        },
                        ""aggregates"":[  
                            {  
                                ""dimension"":{  
                                    ""dateHistogram"":{  
                                        ""input"":{  
                                            ""builtInProperty"":""$ts""
                                        },
                                        ""breaks"":{  
                                            ""size"":""1m""
                                        }
                                    }
                                },
                                ""measures"":[  
                                    {  
                                        ""count"":{}
                                    }
                                ]                       
                            }
                        ]
                    }
                }";

            Uri wssAggregatesUri = new UriBuilder("wss", EnvironmentFqdn)
            {
                Path = "/aggregates",
                Query = "api-version=2016-12-12"
            }.Uri;

            var wssAggregatesResponse = await AsyncWssHelper(socket, wssAggregatesUri, wssAggregatesInput);
            */

        }

        static void Main(string[] args)
        {
            Task.Run(async () => await TsiGaSample()).Wait();
        }
    }
}
