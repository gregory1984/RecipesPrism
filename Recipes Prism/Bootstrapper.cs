using Microsoft.Practices.Unity;
using Prism.Unity;
using Recipes_Prism.Views;
using Recipes_Prism.Helpers;
using System.Windows;
using Recipes_Model.Interfaces;
using Recipes_Model.Services;

namespace Recipes_Prism
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            Container.RegisterType<IDatabaseService, DatabaseService>();
            Container.RegisterType<IDictionariesService, DictionariesService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IMountingService, MountingService>();
            Container.RegisterType<IOrderingService, OrderingService>();
            Container.RegisterInstance(UnityNames.VersionData, new VersionData(), new ContainerControlledLifetimeManager());
            Container.RegisterType<IBackupService, BackupService>();

            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
    }
}
