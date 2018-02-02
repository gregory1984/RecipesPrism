using System.Windows;
using MahApps.Metro.Controls;
using Recipes_Prism.ViewModels;
using Recipes_Prism.Helpers;

namespace Recipes_Prism.Views
{
    /// <summary>
    /// Interaction logic for DictionaryEditWindow.xaml
    /// </summary>
    public partial class DictionaryEditWindow : MetroWindow
    {
        public DictionaryEditWindow()
        {
            InitializeComponent();

            var viewmodel = DataContext as DictionaryEditWindowViewModel;

            viewmodel.ExceptionOccuredAction += (ex) =>
            {
                MessageBoxes.CriticalQuestion("Wystąpił krytyczny wyjątek. Czy chcesz zobaczyć szczegóły?", ex.ToString(), "Błąd krytyczny");
            };

            viewmodel.DictionaryUpdatedAction += (selectedDictionaryType) =>
            {
                MessageBoxes.Information(string.Format("Aktualizacja {0} zakończona.", selectedDictionaryType), "Informacja");
                Close();
            };
        }
    }
}
