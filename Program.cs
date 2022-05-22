using System;
using System.Threading.Tasks;
using System.Net.Http;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using AssertLibrary;

namespace RestSharpTest
{
    class Program
    {
        
 //       private static readonly HttpClient client = new HttpClient();

        private static async Task ProcessRepositories()
        {
            
            string url = "https://petstore.swagger.io/v2";
            
            // Read Test data from file. The file location is relative to the executable (file), that is why the file is not directly read from a 
            // predefined location
            string currentDirectory =  Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase); 
            Console.WriteLine("Current Directory : "+currentDirectory);
            
            string directory = currentDirectory.Substring(currentDirectory.IndexOf("\\")+1,currentDirectory.Length-10);

            Console.WriteLine("Current Directory : "+directory);
            
            StreamReader r = new StreamReader("content.json");
            string jsonString = r.ReadToEnd();

            var restclient = new RestSharp.RestClient(url);           
            // Define GET endpoint
            var request = new RestSharp.RestRequest("/pet/findByStatus?status=available", Method.Get);            
            request.AddHeader("Accept", "application/json");             
            //Execute GET operation
            var response = await  restclient.ExecuteAsync<string>(request);
            // Print the response
            Console.WriteLine(response.Content);            
            int StatusCode = (int)response.StatusCode;            
            Assert.IsEqual(200,StatusCode, "Status code is not 200");

            // Define POST endpoint
            request = new RestSharp.RestRequest("/pet", Method.Post);                        
            // Define post parameters and give data
            request.AddHeader("Accept", "application/json");                                             
            request.AddStringBody(jsonString,"application/json");                            
            //Execute POST
          
            response = await  restclient.ExecuteAsync<string>(request);                     
            // Print the response
            Console.WriteLine(response.Content);
            StatusCode = (int)response.StatusCode;            
            Assert.IsEqual(200,StatusCode, "Status code is not 200");

            // Modify the data by setting the status to "sold" 
            JObject resp = JObject.Parse(response.Content);            
            JObject data = JObject.Parse(jsonString);            
            data["status"]="sold";
            data["id"]=resp["id"];
            
       
            // Define PUT endpoint
            request = new RestSharp.RestRequest("/pet", Method.Put);

            // Define put parameters and data
            request.AddStringBody(data.ToString(Formatting.None),"application/json");                                    
            request.AddHeader("content-type", "application/json");

            // Execute PUT operation
            response = await  restclient.ExecuteAsync<string>(request);   
            // Print the response
            Console.WriteLine("Put response : "+response.Content);
            StatusCode = (int)response.StatusCode;            
            Assert.IsEqual(200,StatusCode, "Status code is not 200");
            
            // Use the id of the repsonse data as the identifier id for DELETE                
            dynamic responseJson  = JsonConvert.DeserializeObject(response.Content);            
            var id = responseJson.id;
            Console.WriteLine("id : "+data["id"]);
            // Define DELETE endpoint
            string endpoint = "/pet/"+id;
            Console.WriteLine("Delete endpoint : "+endpoint);
            request = new RestSharp.RestRequest(endpoint, Method.Delete);            
            request.AddHeader("Accept", "application/json");             
            Console.WriteLine("Request to string : "+request.ToString());
            // Execute DELETE operation            
            response = await  restclient.ExecuteAsync<string>(request);        
                    
            // Print the response
            Console.WriteLine(response.Content);
            StatusCode = (int)response.StatusCode;            
            Assert.IsEqual(200,StatusCode, "Status code is not 200");

            // Try to query the already deleted pet
            request = new RestSharp.RestRequest("/pet/"+data["id"], Method.Get);            
            request.AddHeader("Accept", "application/json");             
            //Execute GET operation
            response = await  restclient.ExecuteAsync<string>(request);
            // Print the response
            Console.WriteLine(response.Content);            

            StatusCode = (int)response.StatusCode;            
            Assert.IsEqual(404,StatusCode, "Status code is not 404");
            

        }
        
        static async Task Main(string[] args)
        {
            await ProcessRepositories();
        }
    }
}
