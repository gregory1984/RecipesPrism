using System.Windows;
using Recipes_Prism.Helpers;
using MahApps.Metro.Controls;
using Recipes_Prism.ViewModels;
using Recipes_Prism.Enums;

namespace Recipes_Prism.Views
{
    /// <summary>
    /// Interaction logic for AddEditOrderWindow.xaml
    /// </summary>
    public partial class AddEditOrderWindow : MetroWindow
    {
        public AddEditOrderWindow()
        {
            InitializeComponent();

            var viewmodel = DataContext as AddEditOrderWindowViewModel;

            viewmodel.OrderAddedEditedAction += (orderName, windowMode)
                  => MessageBoxes.Information(string.Format("Zlecenie [ {0} ] zostało {1}.", orderName, windowMode == WindowMode.Addition ? "dodane" : "zmienione"));

            viewmodel.EditOrderQuestionAction += (orderName) =>
            {
                var result = MessageBox.Show(string.Format("Zlecenie [ {0} ] zostanie zmodyfikowane. Kontynuować?", orderName), "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            };

            viewmodel.CloseWindowAction += () => Close();

            Unloaded += (sender, e) => viewmodel.UnsubscribeEvents();
        }
    }
}
