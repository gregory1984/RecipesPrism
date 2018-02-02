using System.Windows.Controls;
using Recipes_Prism.ViewModels;

namespace Recipes_Prism.Views
{
    /// <summary>
    /// Interaction logic for ProductDictionary
    /// </summary>
    public partial class ProductDictionary : UserControl
    {
        public ProductDictionary()
        {
            InitializeComponent();

            var viewmodel = DataContext as ProductDictionaryViewModel;

            Unloaded += (sender, e) => viewmodel.UnsubscribeEvents();
        }
    }
}
