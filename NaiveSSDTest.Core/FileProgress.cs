namespace NaiveSSDTest.Core
{
    public class FileProgress
    {
        public int TotalCount { get; set; }
        public long TotalSize { get; set; }
        public int CurrentCount { get; set; }
        public long CurrentSize { get; set; }

        public bool CheckUpdate(int divider)
        {
            return CurrentCount % divider == 0
                || CurrentCount == 1
                || CurrentCount == TotalCount;
        }
    }
}