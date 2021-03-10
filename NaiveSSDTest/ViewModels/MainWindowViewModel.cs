using JToolbox.Core.Utilities;
using Microsoft.VisualBasic.FileIO;
using NaiveSSDTest.Core;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NaiveSSDTest.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string busyMessage;
        private string messages;
        private DriveViewModel selectedDrive;
        private TestData testData;

        public MainWindowViewModel(TestData testData)
        {
            this.testData = testData;
            Drives = new ObservableCollection<DriveViewModel>(
                DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed)
                .Select(d => new DriveViewModel(d)));
            SelectedDrive = Drives.First();
        }

        public ObservableCollection<DriveViewModel> Drives { get; set; }

        public DriveViewModel SelectedDrive
        {
            get => selectedDrive;
            set => SetProperty(ref selectedDrive, value);
        }

        public bool IsBusy { get; set; }

        public string BusyMessage
        {
            get => busyMessage;
            set
            {
                SetProperty(ref busyMessage, value);
                IsBusy = !string.IsNullOrEmpty(value);
                RaisePropertyChanged(nameof(IsBusy));
            }
        }

        public string Messages
        {
            get => messages;
            set
            {
                messages = value + Environment.NewLine + messages;
                RaisePropertyChanged(nameof(Messages));
            }
        }

        public DelegateCommand RunCommand => new DelegateCommand(async () =>
        {
            var targetPath = string.Empty;
            try
            {
                BusyMessage = "Preparing test data. Please wait...";
                var path = SelectedDrive.DriveInfo.RootDirectory.FullName;
                Messages = $"Preparing test data for {path}";
                await Task.Run(() => testData.CreateTestDataStructure(path));
                Messages = $"Test data files: {testData.TotalFiles}, {Misc.BytesToString(testData.TotalSize)}";

                BusyMessage = "Test started. Please wait...";
                await Task.Delay(2000);
                var sourcePath = Path.Combine(path, testData.Folder);
                targetPath = Path.Combine(path, testData.Folder + "_COPY");
                var stopwatch = Stopwatch.StartNew();
                Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(sourcePath, targetPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                var elapsed = stopwatch.Elapsed.TotalSeconds;
                var averageSpeed = (long)Math.Round(testData.TotalSize / elapsed);
                Messages = $"Test finished in: {Math.Round(elapsed, 2)}s with average speed: {Misc.BytesToString(averageSpeed)}/s";
            }
            catch (OperationCanceledException)
            {
                Messages = "Test cancelled";
            }
            catch (Exception exc)
            {
                Messages = exc.Message;
            }
            finally
            {
                BusyMessage = "Cleaning up...";
                if (Directory.Exists(targetPath))
                {
                    Directory.Delete(targetPath, true);
                }
                BusyMessage = null;
            }
        });
    }
}