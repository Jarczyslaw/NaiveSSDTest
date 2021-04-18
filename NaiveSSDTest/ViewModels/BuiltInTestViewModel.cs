using JToolbox.Core.Utilities;
using NaiveSSDTest.Core;
using NaiveSSDTest.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NaiveSSDTest.ViewModels
{
    public abstract class BuiltInTestViewModel : TestViewModel
    {
        public BuiltInTestViewModel(MainWindowViewModel main, CopyManager copyManager, DataManager dataManager, ProgressService progressService)
            : base(main, copyManager, dataManager, progressService)
        {
        }

        public abstract bool Random { get; }

        public override async Task Run(Configuration configuration)
        {
            await PrepareRun(configuration);
            copyManager.Random = Random;
            var stopwatch = Stopwatch.StartNew();
            var filesToCopy = progressService.RunCopyProcess(configuration, copyManager);
            var elapsed = stopwatch.Elapsed.TotalSeconds;
            var averageSpeed = (long)Math.Round(filesToCopy.Size / elapsed);
            main.Messages = $"Test finished in: {Math.Round(elapsed, 2)}s with average speed: {Misc.BytesToString(averageSpeed)}/s";
        }

        public override async Task Clean()
        {
            await Task.Run(() => copyManager.Cleaner.Cleanup());
        }
    }
}