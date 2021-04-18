using JToolbox.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NaiveSSDTest.Core
{
    public abstract class FilesProcessingQueue : ProcessingQueue<TestFile, bool>
    {
        private int totalFiles;
        private int filesProcessed;
        private long totalSize;
        private long sizeProcessed;
        private readonly object progressLock = new object();
        protected Configuration configuration;

        public FilesProcessingQueue(Configuration configuration)
        {
            this.configuration = configuration;
            StopOnFirstException = true;
        }

        public virtual async Task<List<ProcessingQueueItem<TestFile, bool>>> ProcessFiles(List<TestFile> files, CancellationToken cancellationToken = default)
        {
            totalFiles = files.Count;
            totalSize = files.Sum(f => f.FileSize);
            filesProcessed = 0;
            sizeProcessed = 0;
            ProcessingStart(GetFileProgress());
            var result = await Run(files, cancellationToken);
            ProcessingFinish();
            return result;
        }

        public abstract void ProcessingStart(FileProgress fileProgress);

        public abstract void ProcessingFinish();

        public abstract void ReportFileProgress(FileProgress fileProgress);

        public override Task ReportProgress(ProcessingQueueItem<TestFile, bool> item)
        {
            if (item.Output)
            {
                lock (progressLock)
                {
                    filesProcessed++;
                    sizeProcessed += item.Input.FileSize;
                    ReportFileProgress(GetFileProgress());
                }
            }
            return Task.CompletedTask;
        }

        private FileProgress GetFileProgress()
        {
            return new FileProgress
            {
                TotalSize = totalSize,
                TotalCount = totalFiles,
                CurrentCount = filesProcessed,
                CurrentSize = sizeProcessed
            };
        }
    }
}