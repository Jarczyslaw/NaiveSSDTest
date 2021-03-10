using JToolbox.Core.Utilities;
using System;

namespace NaiveSSDTest.Core
{
    internal class TestDataEntry
    {
        public int Index { get; set; }
        public long FileSize { get; set; }
        public int FilesCount { get; set; }

        public long TotalSize => FileSize * FilesCount;

        public string Folder => $"TEST_FILES_{Index}_{Misc.BytesToString(FileSize)}";

        
    }
}