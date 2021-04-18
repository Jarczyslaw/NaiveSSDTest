using JToolbox.Core.Utilities;
using NaiveSSDTest.Core;
using System;
using System.Threading.Tasks;

namespace NaiveSSDTest.ViewModels
{
    public class FileCreateProgressViewModel : ProgressViewModel, IFilesCreationHandler
    {
        private DataManager dataManager;
        private Configuration configuration;

        public FileResult Result { get; set; }

        public FileCreateProgressViewModel(Configuration configuration, DataManager dataManager)
        {
            this.configuration = configuration;
            this.dataManager = dataManager;

            Indeterminated = false;
            Label = "Creating files structure...";
            ProgressValue = 0;
        }

        public override async Task Run()
        {
            Result = await dataManager.CreateData(configuration, this);
        }

        public void FileCreated(FileProgress progress)
        {
            ProgressInfo = $"File: {progress.CurrentCount}/{progress.TotalCount}, size: {Misc.BytesToString(progress.CurrentSize)}/{Misc.BytesToString(progress.TotalSize)}";
            ProgressValue = Math.Round(progress.CurrentCount * 100d / progress.TotalCount);
        }

        public void FilesCreationFinished()
        {
            ProgressValue = 100;
        }

        public void FilesCreationStarted(FileProgress progress)
        {
            ProgressValue = 0;
            ProgressInfo = $"Creating {progress.TotalCount} files with size: {Misc.BytesToString(progress.TotalSize)}";
        }
    }
}