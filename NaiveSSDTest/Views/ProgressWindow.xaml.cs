using NaiveSSDTest.ViewModels;
using System.Threading.Tasks;
using System.Windows;

namespace NaiveSSDTest.Views
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        private ProgressViewModel progressViewModel;
        private bool canClose;

        public ProgressWindow(ProgressViewModel progressViewModel)
        {
            this.progressViewModel = progressViewModel;
            DataContext = progressViewModel;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await progressViewModel.Run();
            canClose = true;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !canClose;
        }
    }
}