using System.Windows.Controls;
using Recipes_Prism.ViewModels;

namespace Recipes_Prism.Views
{
    /// <summary>
    /// Interaction logic for ComponentsOfProduct
    /// </summary>
    public partial class ComponentsOfProduct : UserControl
    {
        public ComponentsOfProduct()
        {
            InitializeComponent();

            var viewmodel = DataContext as ComponentsOfProductViewModel;

            Unloaded += (sender, e) => viewmodel.UnsubscribeEvents();
        }
    }
}
