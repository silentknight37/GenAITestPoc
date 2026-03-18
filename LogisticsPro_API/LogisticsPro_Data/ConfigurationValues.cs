
using Microsoft.Extensions.Configuration;

namespace LogisticsPro_Data
{
    public class ConfigurationValues
    {
        private static IConfigurationRoot? _config;

        public static IConfiguration Configuration
        {
            get
            {
                if(_config != null)
                {
                    return _config;
                }

                var builder = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json",
                    optional: true,
                    reloadOnChange: true);

                return _config= builder.Build();
            }
        }


        public static string DATABASE_CONNECTION_STRING = Configuration.GetValue<string>("ConnectionStrings:YoutubeShare_DB_Connection_String");
        public static int DATABASE_TIMEOUT = Configuration.GetValue<int>("ConnectionTimeOut:YoutubeShare_DB_Connection_TimeOut");
    }
}
