using NaiveSSDTest.Core;
using NaiveSSDTest.ViewModels;
using NaiveSSDTest.Views;
using System.Windows;

namespace NaiveSSDTest.Services
{
    public class ProgressService
    {
        public FileResult RunCreationProcess(Configuration configuration, DataManager dataManager)
        {
            var viewModel = new FileCreateProgressViewModel(configuration, dataManager);
            var window = new ProgressWindow(viewModel)
            {
                Owner = Application.Current.MainWindow
            };
            window.ShowDialog();
            return viewModel.Result;
        }

        public FileResult RunCopyProcess(Configuration configuration, CopyManager copyManager)
        {
            var viewModel = new FileCopyProgressViewModel(configuration, copyManager);
            var window = new ProgressWindow(viewModel)
            {
                Owner = Application.Current.MainWindow
            };
            window.ShowDialog();
            return viewModel.Result;
        }
    }
}