using System.Windows.Controls;
using Recipes_Prism.ViewModels;

namespace Recipes_Prism.Views
{
    /// <summary>
    /// Interaction logic for MainWindowFilters
    /// </summary>
    public partial class MainWindowFilters : UserControl
    {
        public MainWindowFilters()
        {
            InitializeComponent();

            var viewmodel = DataContext as MainWindowFiltersViewModel;

            Unloaded += (sender, e) => viewmodel.UnsubscribeEvents();
        }
    }
}
