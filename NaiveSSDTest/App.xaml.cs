using NaiveSSDTest.Core;
using NaiveSSDTest.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace NaiveSSDTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private TestData testData;

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            testData = new TestData();
            testData.CreateDefaultConfiguration();
            containerRegistry.RegisterInstance(testData);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            testData.Cleanup();
            base.OnExit(e);
        }
    }
}
