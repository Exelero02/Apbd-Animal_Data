
using System.Net;
using System.Text;
using System.Text.Json;

namespace Apbd4
{
    public class Program
    {
        private static readonly List<Animal> Animals = new List<Animal>();
        private static readonly List<Visit> Visits = new List<Visit>();
        private static readonly HttpListener Listener = new HttpListener();

        public static async Task Main(string[] args)
        {
            Listener.Prefixes.Add("http://localhost:8080/");
            Listener.Start();
            Console.WriteLine("Listening for requests");

            while (Listener.IsListening)
            {
                var context = await Listener.GetContextAsync();
                ProcessRequest(context);
            }
        }

        private static async void ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            response.ContentType = "application/json";
            string responseString = null;

            if (request.HttpMethod == "GET")
            {
                if (request.Url.AbsolutePath == "/api/animals")
                {
                    responseString = JsonSerializer.Serialize(Animals);
                }
                else if (request.Url.AbsolutePath.StartsWith("/api/animals/"))
                {
                    var animalId = int.Parse(request.Url.AbsolutePath.Substring("/api/animals/".Length));
                    var animal = Animals.Find(a => a.Id == animalId);
                    responseString = animal != null ? JsonSerializer.Serialize(animal) : "Animal not found";
                }
                else if (request.Url.AbsolutePath.StartsWith("/api/visits/"))
                {
                    var animalId = int.Parse(request.Url.AbsolutePath.Substring("/api/visits/".Length));
                    var animalVisits = Visits.FindAll(v => v.AnimalId == animalId);
                    responseString = JsonSerializer.Serialize(animalVisits);
                }
            }
            else if (request.HttpMethod == "POST")
            {
                if (request.Url.AbsolutePath == "/api/animals")
                {
                    var requestBody = await request.InputStream.ReadToEndAsync();
                    var newAnimal = JsonSerializer.Deserialize<Animal>(requestBody);
                    newAnimal.Id = Animals.Count + 1;
                    Animals.Add(newAnimal);
                    responseString = JsonSerializer.Serialize(newAnimal);
                }
                else if (request.Url.AbsolutePath == "/api/visits")
                {
                    var requestBody = await request.InputStream.ReadToEndAsync();
                    var newVisit = JsonSerializer.Deserialize<Visit>(requestBody);
                    Visits.Add(newVisit);
                    responseString = JsonSerializer.Serialize(newVisit);
                }
            }

            var buffer = Encoding.UTF8.GetBytes(responseString ?? "");
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.Close();
        }
    }

    public static class Extensions
    {
        public static async Task<string> ReadToEndAsync(this System.IO.Stream stream)
        {
            using (var reader = new System.IO.StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}