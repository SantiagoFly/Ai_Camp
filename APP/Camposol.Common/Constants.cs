namespace Camposol.Common
{
    /// <summary>
    /// Constants used for in the app
    /// </summary>
    public static class Constants
    {
        public static string DatabaseName = "camposol.db3";

        public static string ContainerName = "audio-files";

        public static string UriBlob = "https://stcamposolpocpaltas.blob.core.windows.net/audio-files?sp=r&st=2024-04-22T17:38:23Z&se=2024-08-02T01:38:23Z&spr=https&sv=2022-11-02&sr=c&sig=RRhXfTWK5OUGe2UVHMYcrXzgLNuRZ4S289H%2BEIwEDXU%3D";

        public static string BlobSAS = "sp=r&st=2024-04-22T17:38:23Z&se=2024-08-02T01:38:23Z&spr=https&sv=2022-11-02&sr=c&sig=RRhXfTWK5OUGe2UVHMYcrXzgLNuRZ4S289H%2BEIwEDXU%3D";

        public static string ContainerConnectionString = "DefaultEndpointsProtocol=https;AccountName=stcamposolpocpaltas;AccountKey=L42gZT6EnQNTx4TnPAdy+5Kj21T1NLzvglzjtKF70WCNKT2QZdrRG+LyQDgCUndfTNI+jzfv0ntg+AStXyB8JA==;EndpointSuffix=core.windows.net";

        public const string BaseUri = "https://";

        public const string SendRecorder = BaseUri + "/api/SendRecorder"; 

        public static string WebApiKeyHeader => "x-functions-key";

        public static string WebApiKey => (IsReleaseEnvironment) ? "WEB_API_KEY_PROPERTY" : "";

        public static string WebApiHost => (IsReleaseEnvironment) ? "WEB_API_HOST_PROPERTY" : "";
     
        public static string EnviromentName => (IsReleaseEnvironment) ? "ENVIRONMENT_NAME_PROPERTY" : "";
       
        public static string AppCenterDroid => (IsReleaseEnvironment) ? "APP_CENTER_DROID_PROPERTY" : "";

        public static string AppCenteriOS => (IsReleaseEnvironment) ? "APP_CENTER_IOS_PROPERTY" : "";

        public static bool IsReleaseEnvironment
        {
            get
            {
#if !DEBUG
                return true;
#else
                return false;
#endif
            }
        }
    }
}
