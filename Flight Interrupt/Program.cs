using Flight_Interrupt;
using System.Data.SqlServerCe;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Flight_Interrupt
{

    class Program
    {
        //initialise APIs
        public static HttpClient client = new HttpClient();
        public static HttpRequestMessage request = new HttpRequestMessage();

        static async Task Main(string[] args)
        {
            //READ API KEYS
            var path = @"\\strs/dfs/Devs/Data/17EDECHCo/! Github/Flight-Interruption-Simulator-due-to-Natural-Disasters/Flight Interrupt/Secrets.txt";
            string[] APIKeys = File.ReadAllLines(path);
            foreach (string line in APIKeys)
            {
                Console.WriteLine(line);
            }

            //Flight Tracker API

            var distance = 250;
            client = new HttpClient();
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://adsbexchange-com1.p.rapidapi.com/v2/lat/51.46888/lon/-0.45536/dist/"+ distance +"/"),
                Headers =
                {
                    { "X-RapidAPI-Key", APIKeys[0] },
                    { "X-RapidAPI-Host", "adsbexchange-com1.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);
            }

            Console.WriteLine();


            //Weather Tracker API
            client = new HttpClient(); 
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/" + Longitude + "%2C%" + Latitude +"?unitGroup=metric&include=current&key="+APIKeys[1]+"&contentType=json"),
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);

                var obj = JsonSerializer.Deserialize<WeatherAPIResponse>(body);

                Console.WriteLine(obj.days[0].windspeed);
                Console.WriteLine(obj.days[0].winddir);
            }

            //SQL
            //connect VS to SQL database
            string connectionString = @"Data Source=\\strs/dfs/Devs/Data/17EDECHCo/! Github/Flight-Interruption-Simulator-due-to-Natural-Disasters/Flight Interrupt/VolcanoDatabase.sdf";
            using SqlCeConnection connection = new SqlCeConnection(connectionString);
            connection.Open();

            //create variables to use
            SqlCeCommand command; //used to read into the database
            SqlCeDataReader dataReader; //used to get data specified by query
            string sql = ""; //contains the sql query

            //give query, send command and receive data
            sql = "select VolcanoName, Longitude, Latitude from VolcanoDatabase where country = 'Japan'"; //TEST QUERY (GIVE ALL VOLCANOS FROM JAPAN)
            command = new SqlCeCommand(sql, connection);
            dataReader = command.ExecuteReader();

            //ouput
            while (dataReader.Read()) 
            {
                Console.WriteLine(dataReader.GetValue(0) + " - " + dataReader.GetValue(1) + ", " + dataReader.GetValue(2));
            }

            //close the objects
            dataReader.Close();
            command.Dispose();
            connection.Close();
        }
    }
}