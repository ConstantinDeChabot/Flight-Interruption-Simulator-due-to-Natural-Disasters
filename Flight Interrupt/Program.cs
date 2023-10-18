using Flight_Interrupt;
using System.Data.SqlServerCe;
using System.Net.Http.Headers;
using System.Text.Json;

//READ API KEYS
var path = @"\\strs/dfs/Devs/Data/17EDECHCo/! Github/Flight-Interruption-Simulator-due-to-Natural-Disasters/Flight Interrupt/Secrets.txt";
string[] APIKeys = File.ReadAllLines(path);
foreach (string line in APIKeys)
{
    Console.WriteLine(line);
}


//Flight Tracker API

var distance = 250;
var client = new HttpClient();
var request = new HttpRequestMessage
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
    RequestUri = new Uri("https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/london/today?unitGroup=metric&include=current&key="+APIKeys[1]+"&contentType=json"),
    //https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/cheltenham/today?unitGroup=metric&include=current&key=64922XN86JX7F7TLDE3MXZ3B3&contentType=json
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
string connectionString = @"Data Source=\\strs/dfs/evs/Data/17EDECHCo/! Github/Flight-Interruption-Simulator-due-to-Natural-Disasters/Flight Interrupt/VolcanoDatabase.sdf";
using SqlCeConnection connection = new SqlCeConnection(connectionString);

connection.Open();
using (SqlCeCommand command = connection.CreateCommand())
{
    command.CommandText = "Select *";

    var result = command.ExecuteScalar();
}