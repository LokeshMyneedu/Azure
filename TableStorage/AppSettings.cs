using Microsoft.Extensions.Configuration;


namespace TableStorage
{
    public  class AppSettings
    {
        public string StorageAccountName { get; set; }
        public string TableName { get; set; }

        public string AccessKey { get; set; }
        public static AppSettings LoadAppSettings()
        {          
            IConfigurationRoot configRoot = new ConfigurationBuilder()
                .AddJsonFile("AppSettings.json")
                .Build();
            AppSettings appSettings = configRoot.Get<AppSettings>();
            return appSettings;
        }       
    }
}
