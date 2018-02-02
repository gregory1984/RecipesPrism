using System.Windows;
using System.Windows.Controls;
using Recipes_Prism.Helpers;
using Recipes_Prism.ViewModels;

namespace Recipes_Prism.Views
{
    /// <summary>
    /// Interaction logic for OrdersGrid
    /// </summary>
    public partial class OrdersGrid : UserControl
    {
        public OrdersGrid()
        {
            InitializeComponent();

            var viewmodel = DataContext as OrdersGridViewModel;

            viewmodel.DeleteOrderQuestionAction += (orderName) =>
            {
                var result = MessageBox.Show($"Zlecenie {orderName} zostanie usunięte. Kontynuować?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            };

            viewmodel.OrderDeletedAction += (orderName) => MessageBoxes.Information($"Zlecenie {orderName} zostało usunięte.");

            viewmodel.ExportToExcelAction += () =>
            {
                var saveDialog = new System.Windows.Forms.SaveFileDialog();
                saveDialog.Filter = "Excel (*.xlsx)|*.xlsx";

                if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return saveDialog.FileName;
                return null;
            };

            viewmodel.ExcelDataExportedAction += (orderName) => MessageBoxes.Information($"Zlecenie {orderName} wyeksportowane prawidłowo.");

            Unloaded += (sender, e) => viewmodel.UnsubscribeEvents();
        }
    }
}
