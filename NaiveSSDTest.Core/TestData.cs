using JToolbox.Core.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NaiveSSDTest.Core
{
    public class TestData
    {
        private List<TestDataEntry> entries;
        private List<string> toCleanup = new List<string>();

        public long TotalSize => entries.Sum(e => e.TotalSize);

        public int TotalFiles => entries.Sum(e => e.FilesCount);

        public string Folder => "TEST_DATA";

        private void AddToCleanup(string path)
        {
            if (!toCleanup.Contains(path))
            {
                toCleanup.Add(path);
            }
        }

        public void AddFiles(int filesCount, long fileSize)
        {
            entries = entries ?? new List<TestDataEntry>();
            var index = entries.Count == 0 ? 1 : entries.Max(e => e.Index) + 1;
            entries.Add(new TestDataEntry
            {
                FilesCount = filesCount,
                FileSize = fileSize,
                Index = index
            });
        }

        public void CreateDefaultConfiguration()
        {
            entries?.Clear();
            AddFiles(1000, 1);
            AddFiles(1000, 8);
            AddFiles(1000, 64);
            AddFiles(1000, 256);
            AddFiles(1000, 1024);
            AddFiles(1000, 8 * 1024);
            AddFiles(1000, 64 * 1024);
            AddFiles(1000, 256 * 1024);
            AddFiles(100, 1024 * 1024);
        }

        public void CreateTestDataStructure(string path)
        {
            var targetPath = Path.Combine(path, Folder);
            AddToCleanup(targetPath);
            if (CheckTestDataStructure(targetPath))
            {
                return;
            }

            BuildNewTestData(targetPath);
        }

        private void BuildNewTestData(string targetPath)
        {
            if (Directory.Exists(targetPath))
            {
                Directory.Delete(targetPath, true);
            }

            Directory.CreateDirectory(targetPath);
            var tasks = new List<Task>();
            foreach (var entry in entries)
            {
                var task = Task.Run(() =>
                {
                    var entryPath = Path.Combine(targetPath, entry.Folder);
                    Directory.CreateDirectory(entryPath);
                    for (var i = 0; i < entry.FilesCount; i++)
                    {
                        FileSystem.CreateRandomFile(Path.Combine(entryPath, $"FILE_{i + 1}.bin"), entry.FileSize);
                    }
                });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        public void Cleanup()
        {
            foreach (var path in toCleanup)
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
        }

        private bool CheckTestDataStructure(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            return files.Length == TotalFiles && files.Sum(f => new FileInfo(f).Length) == TotalSize;
        }
    }
}