using JToolbox.WPF.Core;
using NaiveSSDTest.Core;
using NaiveSSDTest.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NaiveSSDTest.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string title;
        private string busyMessage;
        private string messages;
        private DriveViewModel selectedDrive;
        private TestViewModel selectedTest;
        private readonly AppConfig appConfig;
        private readonly CopyManager copyManager;
        private readonly DataManager dataManager;
        private readonly ProgressService progressService;
        private bool running;

        public MainWindowViewModel(AppConfig appConfig, CopyManager copyManager, DataManager dataManager, ProgressService progressService)
        {
            this.appConfig = appConfig;
            this.copyManager = copyManager;
            this.dataManager = dataManager;
            this.progressService = progressService;

            Title = $"Naive SSD Test ({appConfig.TasksCount} workers)";
            InitializeDrives();
            InitializeTests();

            copyManager.TasksCount = appConfig.TasksCount;
            dataManager.TasksCount = appConfig.TasksCount;
            dataManager.ForceFilesCreation = false;
        }

        public ObservableCollection<DriveViewModel> Drives { get; set; }

        public ObservableCollection<TestViewModel> Tests { get; set; }

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public DriveViewModel SelectedDrive
        {
            get => selectedDrive;
            set => SetProperty(ref selectedDrive, value);
        }

        public TestViewModel SelectedTest
        {
            get => selectedTest;
            set => SetProperty(ref selectedTest, value);
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
                Threading.SafeInvoke(() =>
                {
                    messages += value + Environment.NewLine;
                    RaisePropertyChanged(nameof(Messages));
                });
            }
        }

        private void InitializeDrives()
        {
            Drives = new ObservableCollection<DriveViewModel>(
                   DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed)
                   .Select(d => new DriveViewModel(d)));
            SelectedDrive = Drives.First();
        }

        private void InitializeTests()
        {
            Tests = new ObservableCollection<TestViewModel>
            {
                new BuiltInLinearTestViewModel(this, copyManager, dataManager, progressService),
                new BuiltInRandomTestViewModel(this, copyManager, dataManager, progressService),
                new WindowsDefaultTestViewModel(this, copyManager, dataManager, progressService)
            };
            SelectedTest = Tests.First();
        }

        public bool CheckClose()
        {
            return !running;
        }

        public async Task Cleanup()
        {
            BusyMessage = "Cleaning up...";
            await Task.Run(() =>
            {
                copyManager.Cleaner.Cleanup();
                dataManager.Cleaner.Cleanup();
            });
        }

        public DelegateCommand RunCommand => new DelegateCommand(async () =>
        {
            var targetPath = string.Empty;
            try
            {
                running = true;
                var configuration = new Configuration(SelectedDrive.DriveInfo.RootDirectory.FullName);
                await SelectedTest.Run(configuration);
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
                await SelectedTest.Clean();
                BusyMessage = null;
                running = false;
            }
        });
    }
}