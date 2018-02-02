using System.Windows.Controls;
using Recipes_Prism.ViewModels;

namespace Recipes_Prism.Views
{
    /// <summary>
    /// Interaction logic for MeasureDictionary
    /// </summary>
    public partial class MeasureDictionary : UserControl
    {
        public MeasureDictionary()
        {
            InitializeComponent();

            var viewmodel = DataContext as MeasureDictionaryViewModel;

            Unloaded += (sender, e) => viewmodel.UnsubscribeEvents();
        }
    }
}
