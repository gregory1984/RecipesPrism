using System;
using System.Windows;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using Recipes_Prism.ViewModels;
using Recipes_Prism.Helpers;

namespace Recipes_Prism.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow(IUnityContainer unityContainer)
        {
            InitializeComponent();

            var viewmodel = DataContext as MainWindowViewModel;

            viewmodel.ExceptionOccuredAction += (ex) =>
            {
                MessageBoxes.CriticalQuestion("Wystąpił krytyczny wyjątek. Czy chcesz zobaczyć szczegóły?", ex.ToString(), "Błąd krytyczny");
            };

            viewmodel.ShowDictionariesWindowAction += () => unityContainer.Resolve<DictionariesWindow>().ShowDialog();
            viewmodel.ShowMountingWindowAction += () => unityContainer.Resolve<MountingWindow>().ShowDialog();
            viewmodel.QuitApplicationAction += () => Close();
            viewmodel.ShowBackupWindowAction += () => unityContainer.Resolve<BackupWindow>().ShowDialog();
            viewmodel.ShowAddEditOrderWindowAction += () => unityContainer.Resolve<AddEditOrderWindow>().ShowDialog();
            viewmodel.ShowAboutWindowAction += () => unityContainer.Resolve<AboutWindow>().ShowDialog();
            viewmodel.RestartApplicationAction += () =>
            {
                Closing -= MainWindow_Closing;

                var location = Application.ResourceAssembly.Location;
                Process.Start(location);
                Environment.Exit(0);
            };

            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Czy napewno zamknąć aplikację?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                e.Cancel = true;
        }
    }
}
