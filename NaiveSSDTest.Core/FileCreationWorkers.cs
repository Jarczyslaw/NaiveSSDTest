using JToolbox.Core.Utilities;
using JToolbox.Threading;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NaiveSSDTest.Core
{
    public class FileCreationWorkers : FilesProcessingQueue
    {
        private readonly IFilesCreationHandler handler;

        public FileCreationWorkers(Configuration configuration, IFilesCreationHandler handler, int tasksCount)
            : base(configuration)
        {
            this.handler = handler;
            TasksCount = tasksCount;
        }

        public override Task<List<ProcessingQueueItem<TestFile, bool>>> ProcessFiles(List<TestFile> files, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(configuration.SourcePath))
            {
                Directory.CreateDirectory(configuration.SourcePath);
            }
            return base.ProcessFiles(files, cancellationToken);
        }

        public override void ProcessingFinish()
        {
            handler?.FilesCreationFinished();
        }

        public override void ProcessingStart(FileProgress fileProgress)
        {
            handler?.FilesCreationStarted(fileProgress);
        }

        public override Task<bool> ProcessItem(TestFile item)
        {
            FileSystem.CreateRandomFile(Path.Combine(configuration.SourcePath, item.FileName), item.FileSize);
            return Task.FromResult(true);
        }

        public override void ReportFileProgress(FileProgress fileProgress)
        {
            handler?.FileCreated(fileProgress);
        }
    }
}