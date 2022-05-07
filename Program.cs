using System;
using System.Threading.Tasks;
using System.Net.Http;
using RestSharp;
using RestClient;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using System.IO;


namespace RestSharpTest
{
    class Program
    {
  
        private static readonly HttpClient client = new HttpClient();

        private static async Task ProcessRepositories()
        {
          
            string url = "https://petstore.swagger.io";

            // Read Test data from file
            StreamReader r = new StreamReader("C:\\Users\\hp\\programming\\RestSharp\\content.json");
            string jsonString = r.ReadToEnd();
            
            
            var restclient = new RestSharp.RestClient(url);           

            // Define POST endpoint
            var request = new RestSharp.RestRequest("/v2/pet", Method.Post);            
            
            // Define post parameters and give data
            request.AddHeader("Accept", "application/json");                                             
            request.AddStringBody(jsonString,"application/json");
        
                    
            //Execute POST
            var response = await  restclient.ExecuteAsync<string>(request);                     
            // Print the response
            Console.WriteLine(response.Content);

            // Modify the data by setting the status to "sold" 
            JObject resp = JObject.Parse(response.Content);            
            JObject data = JObject.Parse(jsonString);            
            data["status"]="sold";
            data["id"]=resp["id"];
            
       
            // Define PUT endpoint
            request = new RestSharp.RestRequest("/v2/pet", Method.Put);

            // Define put parameters and data
            request.AddStringBody(data.ToString(Formatting.None),"application/json");                                    
            request.AddHeader("content-type", "application/json");

            // Execute POST operation
            response = await  restclient.ExecuteAsync<string>(request);   
            // Print the response
            Console.WriteLine(response.Content);
            
            // Use the id of the repsonse data as the identifier id for DELETE                
            dynamic responseJson  = JsonConvert.DeserializeObject(response.Content);            
            var id = responseJson.id;
            
            // Define DELETE endpoint
            request = new RestSharp.RestRequest("/v2/pet/"+id, Method.Delete);
            
            // Execute DELETE operation
            response = await  restclient.ExecuteAsync<string>(request);        
            
            // Print the response
            Console.WriteLine(response.Content);
            

        }
        
        static async Task Main(string[] args)
        {
            await ProcessRepositories();
        }
    }
}
