using JToolbox.WPF.Core;
using NaiveSSDTest.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NaiveSSDTest.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool cleanedUp;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            (sender as TextBox).ScrollToEnd();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            if (!cleanedUp)
            {
                e.Cancel = true;
                if (!viewModel.CheckClose())
                {
                    return;
                }

                Task.Run(async () =>
                {
                    await viewModel.Cleanup();
                    cleanedUp = true;
                    Threading.SafeInvoke(Close);
                });
            }
        }
    }
}
