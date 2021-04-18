using JToolbox.Core.Utilities;
using NaiveSSDTest.Core;
using System;
using System.Threading.Tasks;

namespace NaiveSSDTest.ViewModels
{
    public class FileCopyProgressViewModel : ProgressViewModel, IFilesCopyHandler
    {
        private CopyManager copyManager;
        private Configuration configuration;

        public FileResult Result { get; set; }

        public FileCopyProgressViewModel(Configuration configuration, CopyManager copyManager)
        {
            this.configuration = configuration;
            this.copyManager = copyManager;

            Indeterminated = false;
            Label = "Coping files...";
            ProgressValue = 0;
        }

        public void FileCopied(FileProgress progress)
        {
            ProgressInfo = $"File: {progress.CurrentCount}/{progress.TotalCount}, size: {Misc.BytesToString(progress.CurrentSize)}/{Misc.BytesToString(progress.TotalSize)}";
            ProgressValue = Math.Round(progress.CurrentCount * 100d / progress.TotalCount);
        }

        public void FilesCopyFinished()
        {
            ProgressValue = 100;
        }

        public void FilesCopyStarted(FileProgress progress)
        {
            ProgressValue = 0;
            ProgressInfo = $"Copying {progress.TotalCount} files with size: {Misc.BytesToString(progress.TotalSize)}";
        }

        public override async Task Run()
        {
            Result = await copyManager.Copy(configuration, this);
        }
    }
}