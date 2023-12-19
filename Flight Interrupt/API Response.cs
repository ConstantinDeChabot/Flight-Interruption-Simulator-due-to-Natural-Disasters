using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flight_Interrupt
{

    public class WeatherAPIResponse
    {
        public int queryCost { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public string resolvedAddress { get; set; }
        public string address { get; set; }
        public string timezone { get; set; }
        public float tzoffset { get; set; }
        public Day[] days { get; set; }
    }


    public class Day
    {
        public string datetime { get; set; }
        /*public int datetimeEpoch { get; set; }
        public float tempmax { get; set; }
        public float tempmin { get; set; }
        public float temp { get; set; }
        public float feelslikemax { get; set; }
        public float feelslikemin { get; set; }
        public float feelslike { get; set; }
        public float dew { get; set; }
        public float humidity { get; set; }
        public float precip { get; set; }
        public float precipprob { get; set; }
        public float precipcover { get; set; }
        public string[] preciptype { get; set; }
        public float snow { get; set; }
        public float snowdepth { get; set; }
        public float windgust { get; set; }*/
        public float windspeed { get; set; }
        public float winddir { get; set; }
        /*public float pressure { get; set; }
        public float cloudcover { get; set; }
        public float visibility { get; set; }
        public float solarradiation { get; set; }
        public float solarenergy { get; set; }
        public float uvindex { get; set; }
        public float severerisk { get; set; }
        public string sunrise { get; set; }
        public int sunriseEpoch { get; set; }
        public string sunset { get; set; }
        public int sunsetEpoch { get; set; }
        public float moonphase { get; set; }
        public string conditions { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public string[] stations { get; set; }
        public string source { get; set; }*/
    }


    public class FlightAPIResponse
    {
        public Aircraft[] ac { get; set; }
        public string msg { get; set; }
        public long now { get; set; }
        public int total { get; set; }
        public long ctime { get; set; }
        public int ptime { get; set; }
    }

    public class Aircraft
    {
        //public string hex { get; set; }
        //public string type { get; set; }
        public string flight { get; set; }
        public string r { get; set; }
        /*
        public string t { get; set; }
        public int alt_baro { get; set; }
        public int alt_geom { get; set; }
        public float gs { get; set; }
        public int ias { get; set; }
        public int tas { get; set; }
        */
        public float mach { get; set; }
        /*
        public int wd { get; set; }
        public int ws { get; set; }
        public int oat { get; set; }
        public int tat { get; set; }
        public float track { get; set; }
        public float roll { get; set; }
        public float mag_heading { get; set; }
        public float true_heading { get; set; }
        public int baro_rate { get; set; }
        public int geom_rate { get; set; }
        public string squawk { get; set; }
        public string category { get; set; }
        public int nav_altitude_mcp { get; set; }
        */
        public float lat { get; set; }
        public float lon { get; set; }
        /*
        public int nic { get; set; }
        public int rc { get; set; }
        public float seen_pos { get; set; }
        public int version { get; set; }
        public int nac_p { get; set; }
        public int nac_v { get; set; }
        public int sil { get; set; }
        public string sil_type { get; set; }
        public int alert { get; set; }
        public int spi { get; set; }
        public object[] mlat { get; set; }
        public object[] tisb { get; set; }
        public int messages { get; set; }
        public float seen { get; set; }
        public float rssi { get; set; }
        public float dst { get; set; }
        public float dir { get; set; }
        */
    }

}
