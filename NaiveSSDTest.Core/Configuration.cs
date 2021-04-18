using System.IO;

namespace NaiveSSDTest.Core
{
    public class Configuration
    {
        public Configuration(string basePath)
        {
            BasePath = basePath;
        }

        public string BasePath { get; }
        public string SourceFolder => "TEST_DATA";
        public string TargetFolder => "TEST_DATA_COPY";
        public string TestFilePrefix => "TEST_FILE";
        public string SourcePath => Path.Combine(BasePath, SourceFolder);
        public string TargetPath => Path.Combine(BasePath, TargetFolder);
    }
}