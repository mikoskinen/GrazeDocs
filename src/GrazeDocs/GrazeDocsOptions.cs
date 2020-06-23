namespace GrazeDocs
{
    public class GrazeDocsOptions
    {
        public bool LivePreviewEnabled
        {
            get;
            set;
        }

        public string LivePreviewUrl
        {
            get;
            set;
        } = "http://localhost:7552";

        public string PublishFolder
        {
            get;
            set;
        }

        public string InitializeFolder
        {
            get;
            set;
        }

        public bool Publish
        {
            get;
            set;
        }


        public string AddDocumentPath
        {
            get;
            set;
        }

        public string ThemeFolder
        {
            get;
            set;
        }

        public string ConfigurationFile
        {
            get;
            set;
        }

        public string AssetsFolder
        {
            get;
            set;
        }

        public string GrazeBinFolder
        {
            get;
            set;
        }

        public bool VerboseLogging
        {
            get;
            set;
        }
        
        public string RootDirectory
        {
            get;
            set;
        }
    }
}
