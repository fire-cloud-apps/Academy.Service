using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Entity.Common
{
    public class Address
    {
        public string Name { get; set; }
        /// <summary>
        /// Plot No or Door No etc.
        /// </summary>
        public string PlotNo { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public City CityDetails { get; set; }
    }

    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int State_Id { get; set; }
        public string State_Code { get; set; }
        public string State_Name { get; set; }

        public int Country_Id { get; set; }
        public string Country_Code { get; set; }
        public string Country_Name { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string WikiDataId { get; set; }

        public string WikiLink
        {
            get
            {
                return $"https://www.wikidata.org/wiki/" + WikiDataId;
            }
        }

    }

    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Iso3 { get; set; }
        public string Iso2 { get; set; }
        public int Numeric_Code { get; set; }
        public string phone_code { get; set; }
        public string capital { get; set; }
        public string currency { get; set; }
        public string currency_name { get; set; }
        public string currency_symbol { get; set; }
        public string tld { get; set; }
        public string native { get; set; }
        public string region { get; set; }
        public string subregion { get; set; }
        public string timezones { get; set; }
        //public List<Timezone> timezones { get; set; }
        //public Translations translations { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string emoji { get; set; }
        public string emojiU { get; set; }
    }
}
