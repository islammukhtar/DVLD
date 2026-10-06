using System.Configuration;
namespace DVLD.DataAccessLayer
{
    public class clsDataAccessSettings
    {
        //static public string ConnectionString = "server=.;Database=DVLD;User=sa;password=sa123456";
        static public string connstr = ConfigurationManager.AppSettings["connstr"];
    }
}
