using JToolbox.Core.Utilities;
using Microsoft.VisualBasic.FileIO;
using NaiveSSDTest.Core;
using NaiveSSDTest.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NaiveSSDTest.ViewModels
{
    public class WindowsDefaultTestViewModel : TestViewModel
    {
        private Configuration configuration;

        public WindowsDefaultTestViewModel(MainWindowViewModel main, CopyManager copyManager, DataManager dataManager, ProgressService progressService)
            : base(main, copyManager, dataManager, progressService)
        {
        }

        public override string Name => "Windows default";

        public override async Task Run(Configuration configuration)
        {
            this.configuration = configuration;
            await PrepareRun(configuration);
            var filesToCopy = Directory.GetFiles(configuration.SourcePath, "*", System.IO.SearchOption.AllDirectories);
            var stopwatch = Stopwatch.StartNew();
            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(configuration.SourcePath, configuration.TargetPath, UIOption.AllDialogs, UICancelOption.ThrowException);
            var elapsed = stopwatch.Elapsed.TotalSeconds;
            var averageSpeed = (long)Math.Round(filesToCopy.Sum(f => new FileInfo(f).Length) / elapsed);
            main.Messages = $"Test finished in: {Math.Round(elapsed, 2)}s with average speed: {Misc.BytesToString(averageSpeed)}/s";
        }

        public override async Task Clean()
        {
            await Task.Run(() =>
            {
                if (Directory.Exists(configuration.TargetPath))
                {
                    Directory.Delete(configuration.TargetPath, true);
                }
            });
        }
    }
}