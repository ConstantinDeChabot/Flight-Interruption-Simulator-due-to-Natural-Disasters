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

            while (true) //Main loop to keep the user in the program
            {
                Console.Clear();

                //Title screen for program
                Console.WriteLine(" _____ _ _       _     _         ___       _                             _       ____  _           ");
                Console.WriteLine("|  ___| (_) __ _| |__ | |_      |_ _|_ __ | |_ ___ _ __ _ __ _   _ _ __ | |_    / ___|(_)_ __ ___  ");
                Console.WriteLine("| |_  | | |/ _` | '_ \\| __|      | || '_ \\| __/ _ \\ '__| '__| | | | '_ \\| __|   \\___ \\| | '_ ` _ \\ ");
                Console.WriteLine("|  _| | | | (_| | | | | |_       | || | | | ||  __/ |  | |  | |_| | |_) | |_     ___) | | | | | | |");
                Console.WriteLine("|_|   |_|_|\\__, |_| |_|\\__|     |___|_| |_|\\__\\___|_|  |_|   \\__,_| .__/ \\__|   |____/|_|_| |_| |_|");
                Console.WriteLine("           |___/                                                  |_|                              ");

                Console.ReadKey();

                //Main Menu options
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
            

        }

        //-------------------------------------------------------- Controls All Menus --------------------------------------------------------

        static int MenuController(string menuName, string[] menuArray) //controls indexing and formatting of menu
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
                else if (/*tempkey.Key == ConsoleKey.Spacebar ||*/ tempkey.Key == ConsoleKey.Enter) //key the user presses to confirm their choice
                {
                    //Console.WriteLine(" exit loop, index:" + index);
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
        }

        static void DisplayMenu(string menuName, string[] menuArray, int index) //displays the menu and its options to user
        {
            int width = Console.WindowWidth;
            Console.SetCursorPosition((width / 2) - (menuName.Length / 2), 5);
            Console.WriteLine(menuName);
            Console.WriteLine();

            for (int i = 0; i < menuArray.Length; i++) //changes the colour of the selected option
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

            //Console.WriteLine(index);
        } 

        //----------------------------------------------------------- Run Program ------------------------------------------------------------

        public static async Task FlightInterruptProgram() //Main program functions
        {
            double[] longLat = await VolcanoSearch(); 

            Program program = new Program();
            double[] speedDir = await program.WeatherAPI(longLat[0], longLat[1]);

            double[] flightAPIParameters = PlumeCalculator(speedDir[0], speedDir[1], longLat[0], longLat[1]);

            await program.FlightTrackerAPI(flightAPIParameters[0], flightAPIParameters[1], flightAPIParameters[2]);

        }

        public static async Task<double[]> VolcanoSearch() //sql search for volcanos to erupt
        {
            Console.Clear();
            //SQL
            //connect VS to SQL database
            string connectionString = @"Data Source=\\strs/dfs/Devs/Data/17EDECHCo/! Github/Flight-Interruption-Simulator-due-to-Natural-Disasters/Flight Interrupt/VolcanoDatabase.sdf";
            using SqlCeConnection connection = new SqlCeConnection(connectionString);
            connection.Open();

            //create variables to use
            SqlCeCommand command; //used to read into the database
            SqlCeDataReader dataReader; //used to get data specified by query
            string sql = ""; //contains the sql query

            invalidNameInput:
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

                //Console.WriteLine(dataReader.GetValue(0) + ": " + Math.Round(latitude, 2) + "N, " + Math.Round(longitude,2) + "E");
                Console.WriteLine();
                Console.WriteLine("Volcano has been found");
                Console.Write(dataReader.GetValue(0) + ": ");

                if (latitude < 0)
                {
                    Console.Write(Math.Round(Math.Abs(latitude), 2) + "S, ");
                }
                else
                {
                    Console.Write(Math.Round(latitude, 2) + "N, ");
                }
                
                if (longitude < 0)
                {
                    Console.Write(Math.Round(Math.Abs(longitude), 2) + "W");
                }
                else
                {

                    Console.Write(Math.Round(longitude, 2) + "E");
                }
                Console.WriteLine();
                //Console.WriteLine("all good");
            }
            if (invalidInput)
            {
                Console.WriteLine("There seems to be a problem");
                Console.WriteLine("Press 1 to try again");
                Console.WriteLine("Press 2 to go to menu");
                goto invalidNameInput;
            }

            //close the objects
            dataReader.Close();
            command.Dispose();
            connection.Close();

            double[] longLat = { longitude, latitude };
            return longLat;
        }

        // Objective Point: 2.a
        public async Task<double[]> WeatherAPI(double longitude, double latitude) //get weather for volcano
        {
            //get API Keys
            var path = @"\\strs/dfs/Devs/Data/17EDECHCo/! Github/Flight-Interruption-Simulator-due-to-Natural-Disasters/Flight Interrupt/Secrets.txt";
            string[] APIKeys = File.ReadAllLines(path);

            //Weather Tracker API
            string url = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/" + latitude + "%2C" + longitude + "?unitGroup=metric&include=current&key=" + APIKeys[1] + "&contentType=json";
            //Console.WriteLine(url);
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
                
                //Console.WriteLine(body); //write out whole API response for testing

                var obj = JsonSerializer.Deserialize<WeatherAPIResponse>(body); //sort the response
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Weather Values:"); //ouput necessary values

                // Objective Point 2.a.i
                windSpeed = obj.days[0].windspeed / 3.6; // converts speed from km/h to m/s
                windDirection = (obj.days[0].winddir + 180) % 360; // changes bearing from where the wind blows from to where it blows to

                Console.WriteLine("Wind speed: " + Math.Round(windSpeed , 2) + " mps");
                Console.WriteLine("Wind dir: " + Math.Round(windDirection) + "°");
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            Console.WriteLine("Weather API Complete");

            double[] speedDir = { windSpeed, windDirection };
            return speedDir;
        }

        public static double[] PlumeCalculator(double windSpeed, double windDirection, double longitudeDegrees, double latitudeDegrees) //calculate plume
        {
            double[] latitudeLongitude = DegreesToMetres(latitudeDegrees, longitudeDegrees);
            double latitude = latitudeLongitude[0];
            double longitude = latitudeLongitude[1] ;

        timeError:
            Console.WriteLine("Enter length of time for which volcano erupts: (in seconds)");
            double distance = 0;
            try
            {
                distance = windSpeed * Convert.ToInt32(Console.ReadLine()); //objective point 2.b.i

            }
            catch 
            {
                Console.WriteLine("That is not a valid time");
                goto timeError;
            }

            Console.WriteLine();
            Console.WriteLine("---- Calculations ----");
            Console.WriteLine("Total distance: " + distance);

            double newLongitude = 0;
            double newLatitude = 0;

            if (windDirection == 0 || windDirection == 180 || windDirection == 360) //objective point 2.c.i
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
            Console.WriteLine("gradient of plume: "+gradient); //objective point 2.c.ii

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
            Console.WriteLine("circleRadius: " + circleRadius);
            circleRadius = Math.Round(circleRadius / 1852);


            Console.WriteLine("newLongitude + newLatitude");
            Console.WriteLine(newLongitude + " " + newLatitude);

            latitudeLongitude = MetresToDegrees(newLatitude, newLongitude);
            latitudeDegrees = Math.Round(latitudeLongitude[0], 2);
            longitudeDegrees = Math.Round(latitudeLongitude[1], 2);

            Console.WriteLine("latitudeDegrees + longitudeDegrees");
            Console.WriteLine(latitudeDegrees + "N " + longitudeDegrees + "E");

            Console.WriteLine();

            double[] flightAPIParameters = { circleRadius, latitudeDegrees, longitudeDegrees };
            return flightAPIParameters;
        }

        public static double[] DegreesToMetres(double latitude, double longitude)
        {
            Console.WriteLine();
            Console.WriteLine();
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
            Console.WriteLine("Latitude in metres: " + Math.Round(latMetre));

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
            Console.WriteLine("Longitude in metres: " + Math.Round(longMetre));
            double[] latLong = { latMetre, longMetre };
            Console.WriteLine();
            Console.WriteLine();
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

        public async Task FlightTrackerAPI(double radius, double latitude, double longitude) //get flights in interrupt zone
        {
            //get API Keys
            var path = @"\\strs/dfs/Devs/Data/17EDECHCo/! Github/Flight-Interruption-Simulator-due-to-Natural-Disasters/Flight Interrupt/Secrets.txt";
            string[] APIKeys = File.ReadAllLines(path);

            //Flight Tracker API
            client = new HttpClient();
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://adsbexchange-com1.p.rapidapi.com/v2/lat/"+ latitude + "/lon/"+ longitude + "/dist/" + radius + "/"),
                Headers =
                {
                    { "X-RapidAPI-Key", APIKeys[0] },
                    { "X-RapidAPI-Host", "adsbexchange-com1.p.rapidapi.com" },
                },
            };

            var response = await client.SendAsync(request);
            string flightNumber;
            string flightRegistration;
            double flightLatitude;
            double flightLongitude;
            int numberOfFlights;
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();

                //Console.WriteLine(body);//write out whole API response for testing
                Console.WriteLine();

                var obj = JsonSerializer.Deserialize<FlightAPIResponse>(body); //sort the response
                numberOfFlights = obj.total;
                if(numberOfFlights == 0)
                {
                    Console.WriteLine("There are currently no flights that would be interrupted");
                }
                else
                {
                    Console.WriteLine("--- There are " + numberOfFlights + " planes in the interrupt area ---");
                    Console.WriteLine();

                    for (int i = 0; i < numberOfFlights; i++)
                    {
                        Console.WriteLine("---- Plane: " + (i + 1) + " ----");

                        flightNumber = obj.ac[i].flight;
                        Console.WriteLine("Flight: " + flightNumber);
                        flightRegistration = obj.ac[i].r;
                        Console.WriteLine("Registration: " + flightRegistration);
                        flightLatitude = obj.ac[i].lat;
                        flightLongitude = obj.ac[i].lon;
                        Console.WriteLine("Position: " + latitude + "N, " + longitude + "S");
                        Console.WriteLine();
                    }
                }


            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            Console.WriteLine("Flight API Complete");
            /*
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);
            }
            */
            Console.ReadKey();
        }
        
        //------------------------------------------------------------- Database -------------------------------------------------------------

        public static void EditDatabaseMenu() //display database, add record, update record, delete record, return to menu
        {
            Console.Clear();
            string[] databaseMenuArray = { ">> Display database <<", ">>    Add record    <<", ">>  Update record   <<", ">>  Delete record   <<", ">>  Return to Menu   <<" };
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
                    UpdateRecord();
                    break;
                case 3:
                    Console.WriteLine("Delete record");
                    DeleteRecord();
                    break;
                case 4:
                    Console.WriteLine("Return to Menu");
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
                database += dataReader.GetValue(0).ToString() + '\t' + " ";
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

            dataReader.Close();
            command.Dispose();
            connection.Close();

            Console.WriteLine();
            Console.WriteLine("Press any key to return to main menu");
            Console.ReadKey();
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
            Console.WriteLine("Latitude (In terms of N):");
            string latitude = Console.ReadLine();
            Console.WriteLine("Longitude (In terms of E:");
            string longitude = Console.ReadLine();
            Console.WriteLine("Altitude:");
            string altitude = Console.ReadLine();
            Console.WriteLine("VEI:");
            string vei = Console.ReadLine();

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
            Console.WriteLine("function addRecord() finished");

            command.Dispose();
            connection.Close();

            Console.WriteLine();
            Console.WriteLine("Press any key to return to main menu");
            Console.ReadKey();
        }

        public static void UpdateRecord()
        {
            Console.Clear();
            Console.WriteLine("Update Record");
            Console.WriteLine();

            Console.WriteLine("----- Input Values -----");
            Console.WriteLine("Current VolcanoID:");
            string volcanoID = Console.ReadLine();
            Console.WriteLine("New VolcanoName:");
            string volcanoName = Console.ReadLine();
            Console.WriteLine("New Type:");
            string type = Console.ReadLine();
            Console.WriteLine("New Country:");
            string country = Console.ReadLine();
            Console.WriteLine("New Latitude (In terms of N):");
            string latitude = Console.ReadLine();
            Console.WriteLine("New Longitude (In terms of E:");
            string longitude = Console.ReadLine();
            Console.WriteLine("New Altitude:");
            string altitude = Console.ReadLine();
            Console.WriteLine("New VEI:");
            string vei = Console.ReadLine();

            string connectionString = @"Data Source=\\strs/dfs/Devs/Data/17EDECHCo/! Github/Flight-Interruption-Simulator-due-to-Natural-Disasters/Flight Interrupt/VolcanoDatabase.sdf";
            SqlCeConnection connection = new SqlCeConnection(connectionString);
            connection.Open();

            SqlCeCommand command;
            SqlCeDataReader dataReader;
            //string sql = "update [VolcanoDatabase] (VolcanoID, VolcanoName, Type, Country, Latitude, Longitude, Altitude, VEI) Values (@volcanoID, @volcanoName, @type, @country, @latitude, @longitude, @altitude, @vei)";
            string sql = "update [VolcanoDatabase] set volcanoName = '"+ volcanoName + "', type = '" + type + "', country = '" + country + "', latitude = " + latitude + ", longitude = "+ longitude + ", Altitude = " + altitude + ", vei = "+ vei + " where VolcanoID = " + volcanoID + ";";
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
                Console.WriteLine("Successfully updated " + volcanoName);
            }
            catch (SqlCeException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            Console.WriteLine("function UpdateRecord() finished");

            command.Dispose();
            connection.Close();

            Console.WriteLine();
            Console.WriteLine("Press any key to return to main menu");
            Console.ReadKey();
        }

        public static void DeleteRecord()
        {
            Console.WriteLine("Delete Record");
            Console.WriteLine();
            Console.WriteLine("Enter the name of the volcano to delete from the database");
            //DELETE FROM table_name WHERE condition;
            string volcanoName = Console.ReadLine();

            string connectionString = @"Data Source=\\strs/dfs/Devs/Data/17EDECHCo/! Github/Flight-Interruption-Simulator-due-to-Natural-Disasters/Flight Interrupt/VolcanoDatabase.sdf";
            SqlCeConnection connection = new SqlCeConnection(connectionString);
            connection.Open();

            SqlCeCommand command;
            SqlCeDataReader dataReader;
            string sql = "delete from VolcanoDatabase where volcanoName = '" + volcanoName + "'";
            command = new SqlCeCommand(sql, connection);
            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Successfully deleted " + volcanoName);
            }
            catch (SqlCeException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            command.Dispose();
            connection.Close();

            Console.WriteLine();
            Console.WriteLine("Press any key to return to main menu");
            Console.ReadKey();
        }
    }
}