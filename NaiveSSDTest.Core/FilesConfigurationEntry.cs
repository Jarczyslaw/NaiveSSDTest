namespace NaiveSSDTest.Core
{
    public class FilesConfigurationEntry
    {
        public FilesConfigurationEntry()
        {
        }

        public FilesConfigurationEntry(int count, long size)
        {
            FilesCount = count;
            FileSize = size;
        }

        public long FileSize { get; set; }
        public int FilesCount { get; set; }

        public long TotalSize => FileSize * FilesCount;
    }
}