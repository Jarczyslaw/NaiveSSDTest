using JToolbox.Core.Utilities;
using NaiveSSDTest.Core;
using NaiveSSDTest.Services;
using System.Threading.Tasks;

namespace NaiveSSDTest.ViewModels
{
    public abstract class TestViewModel
    {
        protected MainWindowViewModel main;
        protected CopyManager copyManager;
        protected DataManager dataManager;
        protected ProgressService progressService;

        public TestViewModel(MainWindowViewModel main, CopyManager copyManager, DataManager dataManager, ProgressService progressService)
        {
            this.main = main;
            this.copyManager = copyManager;
            this.dataManager = dataManager;
            this.progressService = progressService;
        }

        public abstract string Name { get; }

        public abstract Task Run(Configuration configuration);

        public abstract Task Clean();

        public async Task<FileResult> PrepareRun(Configuration configuration)
        {
            var path = main.SelectedDrive.DriveInfo.RootDirectory.FullName;
            main.Messages = $"Preparing test data for {path}";
            var result = progressService.RunCreationProcess(configuration, dataManager);
            if (result.Count == 0)
            {
                main.Messages = "Data creation skipped";
            }
            else
            {
                main.Messages = $"Created {result.Count} files with size: {Misc.BytesToString(result.Size)}";
            }
            var delay = 5;
            main.BusyMessage = $"Test will start in {delay} seconds. Please wait...";
            await Task.Delay(delay * 1000);
            main.BusyMessage = null;
            return result;
        }
    }
}