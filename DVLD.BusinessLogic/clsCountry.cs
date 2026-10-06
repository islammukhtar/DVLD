using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD.DataAccessLayer;

namespace DVLD.BusinessLogic
{
    public class clsCountry
    {
        public int ID { get; set; }
        public string CountryName { get; set; }
        clsCountry(int ID, string CountryName)
        {
            this.ID = ID;
            this.CountryName = CountryName;

        }
        public clsCountry()
        {
            ID = -1;
            CountryName = "";
        }
        public static clsCountry Find(int ID)
        {
            string CountryName = "";

            if (clsCountryData.GetCountryInfoByID(ID, ref CountryName))
            {
                return new clsCountry(ID, CountryName);
            }
            return null;
        }
        public static clsCountry Find(string CountryName)
        {
            int ID = -1;

            if (clsCountryData.GetCountryInfoByName(CountryName, ref ID))
            {
                return new clsCountry(ID, CountryName);
            }
            return null;
        }
        public static DataTable GetAllCountries()
        {
            return clsCountryData.GetAllCountries();
        }

    }
}
