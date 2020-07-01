using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FG_Stream_Helper
{
    public static class ApiHelper
    {
        static string auth = "c456df85c8166322cae64979a9c77353";

        public static GraphQLHttpClient client = new GraphQLHttpClient("https://api.smash.gg/gql/alpha", new NewtonsoftJsonSerializer());
        public static float floo = 1f;

        static readonly Lazy<GraphQLHttpClient> _clientHolder = new Lazy<GraphQLHttpClient>(CreateGraphQLClient);

        static GraphQLHttpClient Client => _clientHolder.Value;

        public static async Task<SmashGGInfoModel> GetSetInfo(string sId)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                        query set($setId: ID!)
                        {
                            set(id: $setId) {
                                id
                                fullRoundText
                                slots {
                                    id
                                    standing {
                                        id
                                        stats {
                                            score {
                                                value
                                            }
                                        }
                                    }
                                    entrant {
                                        id
                                        name
                                    }
                                }
                            }
                        }",
                Variables = new { setId = sId }
            };

            var response = await Client.SendQueryAsync<SmashGGInfoModel>(request);

            return response.Data;
        }



        static GraphQLHttpClient CreateGraphQLClient()
        {
            var client = new GraphQLHttpClient("https://api.smash.gg/gql/alpha", new NewtonsoftJsonSerializer());
            client.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {auth}");
            return client;
        }
    }
}
