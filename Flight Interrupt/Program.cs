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
            Program program = new Program();
            //READ API KEYS
            var path = @"\\strs/dfs/Devs/Data/17EDECHCo/! Github/Flight-Interruption-Simulator-due-to-Natural-Disasters/Flight Interrupt/Secrets.txt";
            string[] APIKeys = File.ReadAllLines(path);
            /* foreach (string line in APIKeys)
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


            
        */
            Console.WriteLine(" _____ _ _       _     _         ___       _                             _       ____  _           ");
            Console.WriteLine("|  ___| (_) __ _| |__ | |_      |_ _|_ __ | |_ ___ _ __ _ __ _   _ _ __ | |_    / ___|(_)_ __ ___  ");
            Console.WriteLine("| |_  | | |/ _` | '_ \\| __|      | || '_ \\| __/ _ \\ '__| '__| | | | '_ \\| __|   \\___ \\| | '_ ` _ \\ ");
            Console.WriteLine("|  _| | | | (_| | | | | |_       | || | | | ||  __/ |  | |  | |_| | |_) | |_     ___) | | | | | | |");
            Console.WriteLine("|_|   |_|_|\\__, |_| |_|\\__|     |___|_| |_|\\__\\___|_|  |_|   \\__,_| .__/ \\__|   |____/|_|_| |_| |_|");
            Console.WriteLine("           |___/                                                  |_|                              ");
            Console.ReadKey();
            string[] mainMenuArray = { ">> Run Program   <<", ">> Edit database <<", ">> Exit program  <<" };
            int mainMenuOption = MenuController("Main Menu", mainMenuArray);
            Console.WriteLine(mainMenuOption);
            switch (mainMenuOption)
            {
                case 0:
                    Console.WriteLine("Run program");
                    await FlightInterruptProgram();
                    break;
                case 1:
                    Console.WriteLine("Edit Database");
                    EditDatabaseMenu();
                    break;
                case 2:
                    Console.WriteLine("Exit Program");
                    Environment.Exit(0);
                    break;
            }

        }

        //-------------------------------------------------------- Controls All Menus --------------------------------------------------------

        static int MenuController(string menuName, string[] menuArray)
        {
            int index = 0;
            Console.Clear();
            DisplayMenu(menuName, menuArray, index);
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

                DisplayMenu(menuName, menuArray, index);
                Console.SetCursorPosition(0, 11);
            }
        } //controls indexing and formatting of menu

        static void DisplayMenu(string menuName, string[] menuArray, int index)
        {
            int width = Console.WindowWidth;
            Console.SetCursorPosition((width / 2) - (menuName.Length / 2), 5);
            Console.WriteLine(menuName);
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
        } //displays the menu and its options to user

        //----------------------------------------------------------- Run Program ------------------------------------------------------------

        public static async Task FlightInterruptProgram()
        {
            double[] longLat = await VolcanoSearch();

            Program program = new Program();
            await program.WeatherAPI(longLat[0], longLat[1]);


        }
        public static async Task<double[]> VolcanoSearch() //sql search for volcanos to erupt
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
            double longitude = 0;
            double latitude = 0;

            //give query, send command and receive data
            sql = "select VolcanoName, Longitude, Latitude from VolcanoDatabase where VolcanoName = '" + volcanoName + "'";
            command = new SqlCeCommand(sql, connection);
            dataReader = command.ExecuteReader();

            //ouput
            bool invalidInput = true;
            while (dataReader.Read()) 
            {
                invalidInput = false;
                longitude = Convert.ToDouble(dataReader.GetValue(1));
                latitude = Convert.ToDouble(dataReader.GetValue(2));
                Console.WriteLine(dataReader.GetValue(0) + ": " + latitude + "N, " + longitude + "E");
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

            double[] longLat = { longitude, latitude };
            return longLat;
        }

        public async Task<double[]> WeatherAPI(double longitude, double latitude) //get weather for volcano
        {
            //get API Keys
            var path = @"\\strs/dfs/Devs/Data/17EDECHCo/! Github/Flight-Interruption-Simulator-due-to-Natural-Disasters/Flight Interrupt/Secrets.txt";
            string[] APIKeys = File.ReadAllLines(path);

            //Weather Tracker API
            string url = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/" + latitude + "%2C" + longitude + "?unitGroup=metric&include=current&key=" + APIKeys[1] + "&contentType=json";
            Console.WriteLine(url);
            client = new HttpClient(); 
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
            };

            double windSpeed = 0;
            double windDirection = 0;

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(body); //write out whole API response

                var obj = JsonSerializer.Deserialize<WeatherAPIResponse>(body); //sort the response
                
                Console.WriteLine("Output Values:"); //ouput necessary values

                windSpeed = obj.days[0].windspeed;
                windDirection = obj.days[0].winddir;

                Console.WriteLine("wind speed: " + windSpeed + " kph");
                Console.WriteLine("wind dir: " + windDirection + "°");
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            Console.WriteLine("Weather API Complete");

            double[] speedDir = { windSpeed, windDirection };
            return speedDir;
        }

        public static void PlumeCalculator() //calculate plume
        {

        }

        public static void FlightTrackerAPI() //get flights in interrupt zone
        {

        }
        
        //------------------------------------------------------------- Database -------------------------------------------------------------

        public static void EditDatabaseMenu() //display database, add record, update record, delete record
        {
            Console.Clear();
            string[] databaseMenuArray = { ">> Display database <<", ">>    Add record    <<", ">>  Update record   <<", ">>  Delete record   <<" };
            int databaseMenuOption = MenuController("Database Menu", databaseMenuArray);
            Console.WriteLine(databaseMenuOption);
            switch (databaseMenuOption)
            {
                case 0:
                    Console.WriteLine("Display database");
                    DisplayDatabase();
                    break;
                case 1:
                    Console.WriteLine("Add record");
                    break;
                case 2:
                    Console.WriteLine("Update record");
                    break;
                case 3:
                    Console.WriteLine("Delete record");
                    break;

            }
        }

        public static void DisplayDatabase()
        {
            Console.Clear();
            Console.WriteLine("Display database");

            string connectionString = @"Data Source=\\strs/dfs/Devs/Data/17EDECHCo/! Github/Flight-Interruption-Simulator-due-to-Natural-Disasters/Flight Interrupt/VolcanoDatabase.sdf";
            SqlCeConnection connection = new SqlCeConnection(connectionString);
            connection.Open();

            SqlCeCommand command;
            SqlCeDataReader dataReader;
            string sql = "";
            sql = "select VolcanoName, Longitude, Latitude, Country, Type, VEI from VolcanoDatabase";
            command = new SqlCeCommand(sql, connection);
            dataReader = command.ExecuteReader();

            //ouput
            while (dataReader.Read())
            {
                string database = "";
                database += dataReader.GetValue(0).ToString() + '\t';
                for (int i = 1; i < 5; i++)
                {
                    if (dataReader.GetValue(i).ToString().Length < 8)
                    {
                        database += '\t';
                    }
                    database += dataReader.GetValue(i).ToString();
                }
                Console.WriteLine(database);
            }
            Console.WriteLine("all good");


        }


    }
}