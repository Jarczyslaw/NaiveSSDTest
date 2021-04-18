using Prism.Mvvm;
using System.Threading.Tasks;
using System.Windows;

namespace NaiveSSDTest.ViewModels
{
    public class ProgressViewModel : BindableBase
    {
        private bool determinated;
        private string label;
        private string progressInfo;
        private double progressValue;
        private string progressValueDisplay;

        public bool Indeterminated
        {
            get => determinated;
            set => SetProperty(ref determinated, value);
        }

        public string Label
        {
            get => label;
            set => SetProperty(ref label, value);
        }

        public string ProgressInfo
        {
            get => progressInfo;
            set
            {
                SetProperty(ref progressInfo, value);
                ProgressInfoVisibility = string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
                RaisePropertyChanged(nameof(ProgressInfoVisibility));
            }
        }

        public Visibility ProgressInfoVisibility { get; set; }

        public double ProgressValue
        {
            get => progressValue;
            set
            {
                SetProperty(ref progressValue, value);
                ProgressValueDisplay = $"{value}%";
            }
        }

        public string ProgressValueDisplay
        {
            get => progressValueDisplay;
            set => SetProperty(ref progressValueDisplay, value);
        }

        public virtual Task Run()
        {
            return Task.Delay(2000);
        }
    }
}