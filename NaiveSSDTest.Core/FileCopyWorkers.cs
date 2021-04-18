using JToolbox.Threading;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NaiveSSDTest.Core
{
    public class FileCopyWorkers : FilesProcessingQueue
    {
        private readonly IFilesCopyHandler handler;

        public FileCopyWorkers(Configuration configuration, IFilesCopyHandler handler, int tasksCount)
            : base(configuration)
        {
            this.handler = handler;
            TasksCount = tasksCount;
        }

        public override Task<List<ProcessingQueueItem<TestFile, bool>>> ProcessFiles(List<TestFile> files, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(configuration.TargetPath))
            {
                Directory.CreateDirectory(configuration.TargetPath);
            }
            return base.ProcessFiles(files, cancellationToken);
        }

        public override void ProcessingFinish()
        {
            handler?.FilesCopyFinished();
        }

        public override void ProcessingStart(FileProgress fileProgress)
        {
            handler?.FilesCopyStarted(fileProgress);
        }

        public override Task<bool> ProcessItem(TestFile item)
        {
            var sourceFilePath = Path.Combine(configuration.SourcePath, item.FileName);
            var targetFilePath = Path.Combine(configuration.TargetPath, item.FileName);
            File.Copy(sourceFilePath, targetFilePath, true);
            return Task.FromResult(true);
        }

        public override void ReportFileProgress(FileProgress fileProgress)
        {
            handler?.FileCopied(fileProgress);
        }
    }
}