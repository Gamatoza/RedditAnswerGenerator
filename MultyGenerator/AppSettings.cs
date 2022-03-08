using Microsoft.Extensions.Configuration;

namespace MultyGenerator
{
    public static class AppSettings
    {
        private static IConfiguration configuration { get; set; }
        static AppSettings()
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build(); 
        }

        public static int LimitThreadCount => configuration.GetValue<int>("LimitThreadCount");
        public static bool RewriteBrains => configuration.GetValue<bool>("RewriteBrains");

    }
}
