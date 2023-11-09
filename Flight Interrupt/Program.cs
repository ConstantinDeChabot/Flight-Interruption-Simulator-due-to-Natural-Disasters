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
        {/*
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
        */
            VolcanoSearch();
        }

        public static void Menu() //UI menu options: run program, update database, exit program
        {

        }

        public static void VolcanoSearch() //sql search for volcanos to erupt
        {
            //SQL
            //connect VS to SQL database
            string connectionString = @"Data Source=\\strs/dfs/Devs/Data/17EDECHCo/! Github/Flight-Interruption-Simulator-due-to-Natural-Disasters/Flight Interrupt/VolcanoDatabase.sdf";
            using SqlCeConnection connection = new SqlCeConnection(connectionString);
            connection.Open();

            //create variables to use
            SqlCeCommand command; //used to read into the database
            SqlCeDataReader dataReader; //used to get data specified by query
            string sql = ""; //contains the sql query

            Console.WriteLine("Enter name of volcano");
            string volcanoName = Console.ReadLine();

            //give query, send command and receive data
            sql = "select VolcanoName, Longitude, Latitude from VolcanoDatabase where VolcanoName = '" + volcanoName + "'"; //TEST QUERY (GIVE ALL VOLCANOS FROM JAPAN)
            command = new SqlCeCommand(sql, connection);
            dataReader = command.ExecuteReader();

            //ouput
            bool invalidInput = true;
            while (dataReader.Read()) 
            {
                invalidInput = false;
                Console.WriteLine(dataReader.GetValue(0) + ": " + dataReader.GetValue(2) + "N, " + dataReader.GetValue(1) + "E");
                Console.WriteLine("all good");
            }
            if (invalidInput)
            {
                Console.WriteLine("no volcano, there are problems");
            }

            //close the objects
            dataReader.Close();
            command.Dispose();
            connection.Close();
        }

        public static void WeatherAPI() //get weather for volcano
        {

        }

        public static void PlumeCalculator() //calculate plume
        {

        }

        public static void FlightTrackerAPI() //get flights in interrupt zone
        {

        }

        public static void EditDatabase() //display database, add record, update record, delete record
        {

        }
    }
}