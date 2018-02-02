using System.Windows;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using Recipes_Prism.ViewModels;
using Recipes_Prism.Helpers;

namespace Recipes_Prism.Views
{
    /// <summary>
    /// Interaction logic for MountsWindow.xaml
    /// </summary>
    public partial class MountingWindow : MetroWindow
    {
        public MountingWindow(IUnityContainer unityContainer)
        {
            InitializeComponent();

            var viewmodel = DataContext as MountingWindowViewModel;

            viewmodel.ExceptionOccuredAction += (ex) =>
            {
                MessageBoxes.CriticalQuestion("Wystąpił krytyczny wyjątek. Czy chcesz zobaczyć szczegóły?", ex.ToString(), "Błąd krytyczny");
            };

            viewmodel.ShowDictionaryEditWindowAction += () => unityContainer.Resolve<DictionaryEditWindow>().ShowDialog();
            viewmodel.CloseWindowAction += () => Close();

            viewmodel.SelectedProductUnmountQuestionAction += (productName) =>
            {
                var result = MessageBox.Show(string.Format("Czy napewno rozmontować produkt: {0}", productName), "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            };

            viewmodel.SelectedProductMountedAction += (productName) => MessageBoxes.Information(string.Format("Produkt {0} został zmotowany.", productName));
            viewmodel.SelectedProductUnmountedAction += (productName) => MessageBoxes.Information(string.Format("Produkt {0} został rozmontowany.", productName));

            Closing += (sender, e) => viewmodel.UnsubscribePrismEvents();
        }
    }
}
