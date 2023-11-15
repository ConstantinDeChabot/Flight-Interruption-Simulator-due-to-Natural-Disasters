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
            Console.WriteLine(" _____ _ _       _     _         ___       _                             _       ____  _           ");
            Console.WriteLine("|  ___| (_) __ _| |__ | |_      |_ _|_ __ | |_ ___ _ __ _ __ _   _ _ __ | |_    / ___|(_)_ __ ___  ");
            Console.WriteLine("| |_  | | |/ _` | '_ \\| __|      | || '_ \\| __/ _ \\ '__| '__| | | | '_ \\| __|   \\___ \\| | '_ ` _ \\ ");
            Console.WriteLine("|  _| | | | (_| | | | | |_       | || | | | ||  __/ |  | |  | |_| | |_) | |_     ___) | | | | | | |");
            Console.WriteLine("|_|   |_|_|\\__, |_| |_|\\__|     |___|_| |_|\\__\\___|_|  |_|   \\__,_| .__/ \\__|   |____/|_|_| |_| |_|");
            Console.WriteLine("           |___/                                                  |_|                              ");
            Console.ReadKey();
            string[] mainMenuArray = { ">> Run Program   <<", ">> Edit database <<", ">> Exit program  <<" };
            int mainMenuOption = MenuController(mainMenuArray);
            Console.WriteLine(mainMenuOption);
            switch (mainMenuOption)
            {
                case 0:
                    Console.WriteLine("Run program");
                    VolcanoSearch();
                    break;
                case 1:
                    Console.WriteLine("Edit Database");
                    EditDatabase();
                    break;
                case 2:
                    Console.WriteLine("Exit Program");
                    Environment.Exit(0);
                    break;
            }

        }


        static int MenuController(string[] menuArray)
        {
            int index = 0;
            Console.Clear();
            DisplayMenu(menuArray, index);
            while (true) //error handling + wait until user enters one of desired options
            {
                ConsoleKeyInfo tempkey = Console.ReadKey(true);

                if (tempkey.Key == ConsoleKey.UpArrow)
                {
                    index--;
                }
                else if (tempkey.Key == ConsoleKey.DownArrow)
                {
                    index++;
                }
                else if (/*tempkey.Key == ConsoleKey.Spacebar ||*/ tempkey.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(" exit loop, index:" + index);
                    return index;
                }

                Console.Clear();

                Console.SetCursorPosition(5, 10);
                if (index < 0)
                {
                    index += menuArray.Length;
                }
                else
                {
                    index = index % menuArray.Length;
                }

                DisplayMenu(menuArray, index);
                Console.SetCursorPosition(0, 11);
            }
        }

        static void DisplayMenu(string[] menuArray, int index)
        {
            int width = Console.WindowWidth;
            Console.SetCursorPosition((width / 2) - 2, 5);
            Console.WriteLine("Menu");
            Console.WriteLine();

            for (int i = 0; i < menuArray.Length; i++)
            {
                Console.SetCursorPosition((Console.WindowWidth - menuArray[i].Length) / 2, 7 + i);
                if (i == index)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine(menuArray[i]);
                }
                else
                {
                    Console.WriteLine(menuArray[i].Replace('>', ' ').Replace('<', ' '));
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine(index);
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
                Console.WriteLine("There seems to be a problem");
                Console.WriteLine("Press 1 to try again");
                Console.WriteLine("Press 2 to go to menu");
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