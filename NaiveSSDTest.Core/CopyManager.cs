using JToolbox.Core.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JToolbox.Threading;

namespace NaiveSSDTest.Core
{
    public class CopyManager
    {
        public Cleaner Cleaner { get; } = new Cleaner();
        public bool Random { get; set; }

        public int TasksCount { get; set; } = 16;

        public async Task<FileResult> Copy(Configuration configuration, IFilesCopyHandler handler = null)
        {
            var result = new FileResult();
            Cleaner.AddToCleanup(configuration.TargetPath);
            var testFiles = CreateTestFiles(configuration);
            if (Random)
            {
                testFiles.Shuffle();
            }
            var copyWorkers = new FileCopyWorkers(configuration, handler, TasksCount);
            var processedFiles = await copyWorkers.ProcessFiles(testFiles);
            var files = processedFiles.GetProcessedItems().Where(s => s.Output);
            result.Count = files.Count();
            result.Size = files.Sum(s => s.Input.FileSize);
            return result;
        }

        private List<TestFile> CreateTestFiles(Configuration configuration)
        {
            var files = Directory.GetFiles(configuration.SourcePath, "*", SearchOption.AllDirectories);
            return files.Select(f => new TestFile
            {
                FileName = Path.GetFileName(f),
                FileSize = new FileInfo(f).Length
            }).ToList();
        }
    }
}