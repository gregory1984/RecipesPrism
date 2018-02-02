using System.Windows;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using Recipes_Prism.ViewModels;
using Recipes_Prism.Helpers;

namespace Recipes_Prism.Views
{
    /// <summary>
    /// Interaction logic for DictionariesWindow.xaml
    /// </summary>
    public partial class DictionariesWindow : MetroWindow
    {
        public DictionariesWindow(IUnityContainer unityContainer)
        {
            InitializeComponent();

            var viewmodel = DataContext as DictionariesWindowViewModel;

            viewmodel.ExceptionOccuredAction += (ex) =>
            {
                MessageBoxes.CriticalQuestion("Wystąpił krytyczny wyjątek. Czy chcesz zobaczyć szczegóły?", ex.ToString(), "Błąd krytyczny");
            };

            viewmodel.ShowDictionaryEditWindowAction += () => unityContainer.Resolve<DictionaryEditWindow>().ShowDialog();
            viewmodel.CloseWindowAction += () => Close();

            Closing += (sender, e) => viewmodel.UnsubscribePrismEvents();
        }
    }
}
