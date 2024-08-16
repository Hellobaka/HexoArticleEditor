namespace HexoArticleEditor
{
    public static class AppConfig
    {
        public static string HexoBasePath { get; set; } = "";

        public static string HexoArticlePath { get; set; } = "";

        public static string HexoImagePath { get; set; } = "";

        public static string Password { get; set; } = "";

        public static int MaxFileSize { get; set; } = 10 * 1024 * 1024;

        public static bool UseUploadFileName { get; set; } = true;

        public static bool Load()
        {
            try
            {
                HexoBasePath = ConfigHelper.GetConfig("HexoBasePath", "");
                HexoArticlePath = ConfigHelper.GetConfig("HexoArticlePath", "");
                HexoImagePath = ConfigHelper.GetConfig("HexoImagePath", "");
                Password = ConfigHelper.GetConfig("Password", Guid.NewGuid().ToString().Replace("-", ""));
                MaxFileSize = ConfigHelper.GetConfig("MaxFileSize", 10 * 1024 * 1024);
                UseUploadFileName = ConfigHelper.GetConfig("UseUploadFileName", true);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
