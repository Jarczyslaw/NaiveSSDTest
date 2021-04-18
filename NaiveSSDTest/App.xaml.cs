using NaiveSSDTest.Core;
using NaiveSSDTest.Services;
using NaiveSSDTest.Views;
using Prism.Ioc;
using System.Windows;

namespace NaiveSSDTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<CopyManager>();
            containerRegistry.RegisterSingleton<DataManager>();
            containerRegistry.RegisterSingleton<AppConfig>();
            containerRegistry.RegisterSingleton<ProgressService>();
        }
    }
}