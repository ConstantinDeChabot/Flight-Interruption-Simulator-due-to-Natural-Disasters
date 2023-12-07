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
            double[] speedDir = await program.WeatherAPI(longLat[0], longLat[1]);

            PlumeCalculator(speedDir[0], speedDir[1], longLat[0], longLat[1]);

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
                Console.WriteLine(dataReader.GetValue(0) + ": " + Math.Round(latitude, 2) + "N, " + Math.Round(longitude,2) + "E");
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

                windSpeed = obj.days[0].windspeed / 3.6;
                windDirection = (obj.days[0].winddir + 180) % 360;

                Console.WriteLine("wind speed: " + windSpeed + " mps");
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

        public static void PlumeCalculator(double windSpeed, double windDirection, double longitudeDegrees, double latitudeDegrees) //calculate plume
        {
            double[] latitudeLongitude = DegreesToMetres(latitudeDegrees, longitudeDegrees);
            double latitude = latitudeLongitude[0];
            double longitude = latitudeLongitude[1] ;

        timeError:
            Console.WriteLine("Enter length of time for which volcano erupts");
            double distance = 0;
            try
            {
                distance = windSpeed * Convert.ToInt16(Console.ReadLine());

            }
            catch 
            {
                Console.WriteLine("That is not a valid time");
                goto timeError;
            }

            Console.WriteLine("distance: " + distance);

            double newLongitude = 0;
            double newLatitude = 0;

            if (windDirection == 0 || windDirection == 180 || windDirection == 360)
            {
                newLongitude = longitude;
                newLatitude = latitude + (0.5 * distance);
            }
            else if (windDirection == 90 || windDirection == 270)
            {
                newLongitude = longitude + (0.5 * distance);
                newLatitude = latitude;
            }

            double gradient = 1 / Math.Tan(windDirection * Math.PI / 180);//angle needed in radians
            Console.WriteLine("gradient: "+gradient);

            double deltaLongitude = Math.Pow((Math.Pow(0.5 * distance, 2) / Math.Pow(1 + gradient, 2)), 0.5); // deltaX = ( (0.5d)^2 / (1+m)^2 )^0.5
            Console.WriteLine("deltaLongitude: "+deltaLongitude);

            if (windDirection < 180)
            {
                newLongitude = longitude + deltaLongitude;
            }
            else if (windDirection > 180)
            {
                newLongitude = longitude - deltaLongitude;
            }

            if ((windDirection < 90 && windDirection > 0) || (windDirection > 260 && windDirection < 360))
            {
                newLatitude = latitude + (gradient * deltaLongitude);
            }
            else if (windDirection > 90 && windDirection < 270 && windDirection != 180)
            {
                newLatitude = latitude - (gradient * deltaLongitude);
            }

            double circleRadius = distance / 2;

            Console.WriteLine("circleRadius: "+circleRadius);
            Console.WriteLine("newLongitude + newLatitude");
            Console.WriteLine(newLongitude + " " + newLatitude);

            latitudeLongitude = MetresToDegrees(newLatitude, newLongitude);
            latitudeDegrees = Math.Round(latitudeLongitude[0], 2);
            longitudeDegrees = Math.Round(latitudeLongitude[1], 2);

            Console.WriteLine("latitudeDegrees + longitudeDegrees");
            Console.WriteLine(latitudeDegrees + "N " + longitudeDegrees + "E");

        }

        public static double[] DegreesToMetres(double latitude, double longitude)
        {
            int radiusEarth = 6371000;

            //Convert Latitude
            double lat1 = 0;
            double lat2 = latitude * Math.PI / 180;
            double deltaLat = (latitude - 0) * Math.PI / 180;
            double deltaLon = 0;

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                      Math.Cos(lat1) * Math.Cos(lat2) *
                      Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double latMetre = radiusEarth * c; // in metres
            Console.WriteLine("latmetre: " + latMetre);

            //Convert Longtitude
            lat1 = 0;
            lat2 = 0;
            deltaLat = 0 * Math.PI / 180;
            deltaLon = (longitude - 0) * Math.PI / 180;

            a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                      Math.Cos(lat1) * Math.Cos(lat2) *
                      Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double longMetre = radiusEarth * c; // in metres
            Console.WriteLine("lonmetre: " + longMetre);
            double[] latLong = { latMetre, longMetre };
            return latLong;
        }

        public static double[] MetresToDegrees(double latitudeMetres, double longitudeMetres)
        {
            int radiusEarth = 6371000; // Radius of the Earth in meters

            // Calculate latitude in degrees
            double lat1 = 0;
            double lat2 = latitudeMetres / radiusEarth;
            double deltaLat = (latitudeMetres - 0) / radiusEarth;

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(0 / 2) * Math.Sin(0 / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double latitude = (lat2 + lat1) * 180 / Math.PI;

            // Calculate longitude in degrees
            /*c = longitudeMetres / radiusEarth;
            a = Math.Pow(Math.Sqrt(c * c / 4) + Math.Sqrt(1 - Math.Sqrt(c * c / 4)), 2);
            double deltaLon = 2 * Math.Asin(Math.Sqrt(a));
            */

            
            lat1 = 0;
            lat2 = 0;
            deltaLat = 0;
            double deltaLon = longitudeMetres / (radiusEarth * Math.Cos((lat1 + lat2) / 2));

            a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            
            double longitude = (deltaLon + 0) * 180 / Math.PI;
            
            double[] latLong = { latitude, longitude };
            return latLong;
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
                    AddRecord();
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

        public static void AddRecord()
        {
            Console.Clear();
            Console.WriteLine("Add Record");
            Console.WriteLine();

            Console.WriteLine("----- Input Values -----");
            Console.WriteLine("VolcanoID:");
            string volcanoID = Console.ReadLine();
            Console.WriteLine("VolcanoName:");
            string volcanoName = Console.ReadLine();
            Console.WriteLine("Type:");
            string type = Console.ReadLine();
            Console.WriteLine("Country:");
            string country = Console.ReadLine();
            Console.WriteLine("Latitude:");
            string latitude = Console.ReadLine();
            Console.WriteLine("Longitude:");
            string longitude = Console.ReadLine();
            Console.WriteLine("Altitude:");
            string altitude = Console.ReadLine();
            Console.WriteLine("VEI:");
            string vei = Console.ReadLine();



            //Console.WriteLine("add values in form VolcanoID, 'VolcanoName', 'Type', 'Country', Latitude, Longitude, Altitude, VEI");
            //string sqlValues = Console.ReadLine();

            string connectionString = @"Data Source=\\strs/dfs/Devs/Data/17EDECHCo/! Github/Flight-Interruption-Simulator-due-to-Natural-Disasters/Flight Interrupt/VolcanoDatabase.sdf";
            SqlCeConnection connection = new SqlCeConnection(connectionString);
            connection.Open();

            SqlCeCommand command;
            SqlCeDataReader dataReader;
            string sql = "insert into [VolcanoDatabase] (VolcanoID, VolcanoName, Type, Country, Latitude, Longitude, Altitude, VEI) Values (@volcanoID, @volcanoName, @type, @country, @latitude, @longitude, @altitude, @vei)";
            command = new SqlCeCommand(sql, connection);
            command.Parameters.AddWithValue("@volcanoID", volcanoID);
            command.Parameters.AddWithValue("@volcanoName", volcanoName);
            command.Parameters.AddWithValue("@type", type);
            command.Parameters.AddWithValue("@country", country);
            command.Parameters.AddWithValue("@latitude", latitude);
            command.Parameters.AddWithValue("@longitude", longitude);
            command.Parameters.AddWithValue("@altitude", altitude);
            command.Parameters.AddWithValue("@vei", vei);
            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Successfully added " + volcanoName);
            }
            catch (SqlCeException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            Console.WriteLine("Error");
            Console.WriteLine("function addRecord() finished");
        }

    }
}