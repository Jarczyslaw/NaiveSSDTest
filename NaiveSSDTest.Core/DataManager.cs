using JToolbox.Core.Utilities;
using JToolbox.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NaiveSSDTest.Core
{
    public class DataManager
    {
        public Cleaner Cleaner { get; } = new Cleaner();

        public bool ForceFilesCreation { get; set; }

        public int TasksCount { get; set; } = 8;

        private List<FilesConfigurationEntry> CreateDefaultTestFiles()
        {
            return new List<FilesConfigurationEntry>
            {
                new FilesConfigurationEntry(2560, 1),
                new FilesConfigurationEntry(2560, 8),
                new FilesConfigurationEntry(2560, 64),
                new FilesConfigurationEntry(2560, 256),
                new FilesConfigurationEntry(2560, 1024),
                new FilesConfigurationEntry(2560, 8 * 1024),
                new FilesConfigurationEntry(640, 64 * 1024),
                new FilesConfigurationEntry(80, 256 * 1024),
                new FilesConfigurationEntry(20, 1024 * 1024),
                new FilesConfigurationEntry(10, 10 * 1024 * 1024)
            };
        }

        public Task<FileResult> CreateData(Configuration configuration, IFilesCreationHandler handler = null)
        {
            return CreateData(configuration, CreateDefaultTestFiles(), handler);
        }

        public async Task<FileResult> CreateData(Configuration configuration, List<FilesConfigurationEntry> filesConfigurationEntries, IFilesCreationHandler handler = null)
        {
            var result = new FileResult();
            Cleaner.AddToCleanup(configuration.SourcePath);
            if (ForceFilesCreation || !CheckTestDataStructure(configuration.SourcePath, filesConfigurationEntries))
            {
                var testFiles = CreateTestFiles(configuration, filesConfigurationEntries);
                var creationWorkers = new FileCreationWorkers(configuration, handler, TasksCount);
                var processedFiles = await creationWorkers.ProcessFiles(testFiles);
                var files = processedFiles.GetProcessedItems().Where(s => s.Output);
                result.Count = files.Count();
                result.Size = files.Sum(s => s.Input.FileSize);
            }

            return result;
        }

        private int GetTotalFiles(List<FilesConfigurationEntry> filesConfigurationEntries)
        {
            return filesConfigurationEntries.Sum(f => f.FilesCount);
        }

        private long GetTotalSize(List<FilesConfigurationEntry> filesConfigurationEntries)
        {
            return filesConfigurationEntries.Sum(f => f.TotalSize);
        }

        private List<TestFile> CreateTestFiles(Configuration configuration, List<FilesConfigurationEntry> filesConfigurationEntries)
        {
            var files = new List<TestFile>();
            var index = 1;
            var indexLength = GetTotalFiles(filesConfigurationEntries).ToString().Length;
            foreach (var entry in filesConfigurationEntries)
            {
                for (var i = 0; i < entry.FilesCount; i++)
                {
                    var currentSize = Misc.BytesToString(entry.FileSize);
                    files.Add(new TestFile
                    {
                        FileName = $"{configuration.TestFilePrefix}_{index.ToString().PadLeft(indexLength, '0')}_{currentSize}",
                        FileSize = entry.FileSize
                    });
                    index++;
                }
            }
            return files;
        }

        private bool CheckTestDataStructure(string path, List<FilesConfigurationEntry> filesConfigurationEntries)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            return files.Length == GetTotalFiles(filesConfigurationEntries)
                && files.Sum(f => new FileInfo(f).Length) == GetTotalSize(filesConfigurationEntries);
        }
    }
}