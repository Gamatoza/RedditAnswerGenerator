using Microsoft.Extensions.Configuration;

namespace RedditAnswerGenerator.Services
{
    public static class Settings
    {
        private static IConfiguration configuration { get; set; }
        static Settings()
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("settings.json")
                .Build(); 
        }

        public static int CommentLengthMin => configuration.GetValue<int>("CommentLengthMin");
        public static int CommentLengthMax => configuration.GetValue<int>("CommentLengthMax");
        public static int LearnCommentSize => configuration.GetValue<int>("LearnCommentSize");
        public static int LearnRecycleCount => configuration.GetValue<int>("LearnRecycleCount");
        public static string BrainDefaultPath => configuration.GetValue<string>("BrainDefaultPath");
        public static string ReplyDefaultPath => configuration.GetValue<string>("ReplyDefaultPath");
        
    }
}
