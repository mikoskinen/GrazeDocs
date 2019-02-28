namespace GrazeDocs
{
    public class Configuration
    {
        public bool LivePreviewEnabled
        {
            get; set;
        }

        public string LivePreviewUrl { get; set; } = "http://localhost:7552";

        public string LivePreviewPublishFolder
        {
            get; set;
        }

        public string InitializeFolder
        {
            get; set;
        }

        public string AddDocumentPath
        {
            get; set;
        }

        public string ThemeFolder
        {
            get; set;
        }

        public string ConfigurationFile
        {
            get; set;
        }

        public string AssetsFolder
        {
            get; set;
        }

        public string GrazeBinFolder
        {
            get; set;
        }

        public bool VerboseLogging
        {
            get; set;
        }
    }
}
