using NaiveSSDTest.Core;
using NaiveSSDTest.Services;

namespace NaiveSSDTest.ViewModels
{
    public class BuiltInRandomTestViewModel : BuiltInTestViewModel
    {
        public BuiltInRandomTestViewModel(MainWindowViewModel main, CopyManager copyManager, DataManager dataManager, ProgressService progressService)
            : base(main, copyManager, dataManager, progressService)
        {
        }

        public override string Name => "Built-in random";

        public override bool Random => true;
    }
}