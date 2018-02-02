using System.Windows.Controls;
using Recipes_Prism.ViewModels;

namespace Recipes_Prism.Views
{
    /// <summary>
    /// Interaction logic for ComponentDictionary
    /// </summary>
    public partial class ComponentDictionary : UserControl
    {
        public ComponentDictionary()
        {
            InitializeComponent();

            var viewmodel = DataContext as ComponentDictionaryViewModel;

            Unloaded += (sender, a) => viewmodel.UnsubscribeEvents();
        }
    }
}
