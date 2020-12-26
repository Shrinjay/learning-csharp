using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace learningC_
{
    class Program
    {   //Load HTTPClient to send and recieve HTTP requests
        private static readonly HttpClient client = new HttpClient(); 
        static async Task Main(string[] args)
        {
            var repositories = await ProcessRepos(); //Await behaves the same as JS
            foreach (var repo in repositories)
            {
                Console.WriteLine(repo.name);
                Console.WriteLine(repo.GitHubHomeUrl);
                Console.WriteLine(repo.LastPush);
            }
        }

        private static async Task<List<Repository>> ProcessRepos()
        {
            //Sets http request headers using DefaultRequestHeaders
            client.DefaultRequestHeaders.Accept.Clear(); 
            client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            //Sends asynchronous request to get JSON-formatted string. 
            //Old code: 
            /*var stringTask = client.GetStringAsync("https://api.github.com/orgs/dotnet/repos");*/

            //Returns the reponse body as a stream that can be consumed by other functions
            var streamTask = client.GetStreamAsync("https://api.github.com/orgs/dotnet/repos");
            //deserialize the stream being passed in into a list of repository classes, which matches the configuration of the JSON. 
            return await JsonSerializer.DeserializeAsync<List<Repository>>(await streamTask);

            
        }
    }
    //This class contains the name attribute, which the JSON also contains. All other attributes are ignored on deserialization. 
    public class Repository
    {   
        //Maps the name JSON property to name in the C# object. 
        [JsonPropertyName("name")]
        public string name {get; set;}

        [JsonPropertyName("html_url")]
        public Uri GitHubHomeUrl {get; set;}

        [JsonPropertyName("pushed_at")]
        //Returns DateTime object, then defines a new member using => which is an expression for get, which means LastPush is read-only. 
        public DateTime LastPushUtc{get; set;}
        public DateTime LastPush => LastPushUtc.ToLocalTime();
    }
}
