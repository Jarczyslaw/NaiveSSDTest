using NaiveSSDTest.Core;
using NaiveSSDTest.Services;

namespace NaiveSSDTest.ViewModels
{
    public class BuiltInLinearTestViewModel : BuiltInTestViewModel
    {
        public BuiltInLinearTestViewModel(MainWindowViewModel main, CopyManager copyManager, DataManager dataManager, ProgressService progressService)
            : base(main, copyManager, dataManager, progressService)
        {
        }

        public override string Name => "Built-in linear";

        public override bool Random => false;
    }
}