namespace LogisticsPro_API
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
        public static string DATABASE_TIMEOUT = Configuration.GetValue<string>("ConnectionTimeOut:YoutubeShare_DB_Connection_TimeOut");
        public static string CORS_DOMAIN = Configuration.GetValue<string>("CorsSetting:CorsDomain");
        public static string CORS_ORIGINS = Configuration.GetValue<string>("CorsSetting:CorsOrigins");
        public static string API_URL = Configuration.GetValue<string>("APIUrl");
        public static string SECURITY_KEY = Configuration.GetValue<string>("SecurityKey");
    }
}
