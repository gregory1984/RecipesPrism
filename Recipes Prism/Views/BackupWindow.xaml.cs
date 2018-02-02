using System;
using System.Windows;
using System.Diagnostics;
using MahApps.Metro.Controls;
using Recipes_Prism.ViewModels;

namespace Recipes_Prism.Views
{
    /// <summary>
    /// Interaction logic for BackupWindow.xaml
    /// </summary>
    public partial class BackupWindow : MetroWindow
    {
        public BackupWindow()
        {
            InitializeComponent();

            var viewmodel = DataContext as BackupWindowViewModel;

            viewmodel.CloseWindowAction += () => Close();

            viewmodel.DeleteSelectedSnapshotQuestionAction += (snapshotDate) =>
            {
                var dateString = snapshotDate.ToString("dd-MM-yyyy HH:mm:ss");
                var result = MessageBox.Show($"Czy na pewno usunąć migawkę z dnia: {dateString}?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            };

            viewmodel.RecoverSelectedSnapshotQuestionAction += (snapshotDate) =>
            {
                var dateString = snapshotDate.ToString("dd-MM-yyyy HH:mm:ss");
                var result = MessageBox.Show($"Czy na pewno odtworzyć dane migawki z dnia: {dateString}?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            };

            viewmodel.SnapshotCreationFinishedAction += (snapshotFile)
                => MessageBox.Show($"Plik migawki: [ {snapshotFile} ] utworzony.", "", MessageBoxButton.OK, MessageBoxImage.Information);

            viewmodel.SnapshotRecoveringFinishedAction += (snapshotFile)
                => MessageBox.Show($"Baza danych z migawki: [ {snapshotFile} ] przywrócona. Aplikacja zostanie uruchomiona ponownie.", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
