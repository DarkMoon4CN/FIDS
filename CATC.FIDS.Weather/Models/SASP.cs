using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CATC.FIDS.Weather.Models
{
    public class SASP
    {
        public string CCCC { get; set; }

        public string RefTime { get; set; }

        public string WindSpeed { get; set; }

        public string WindDir { get; set; }

        public string GustSpeed { get; set; }

        public string Visprev { get; set; }

        public string Temp { get; set; }

        public string WeatherPhenomena { get; set; }

        public string Cloud { get; set; }

        public string ChangeType { get; set; }

        public string ChangeTypeTime { get; set; }
        
        public string ForecastWindSpeed { get; set; }

        public string ForecastVisprev { get; set; }

        public string ForecastWeatherPhenomena { get; set; }

        public string ForecastCloud { get; set; }

        public string FileName { get; set; }

        public string SearchTime { get; set; }
    }
}