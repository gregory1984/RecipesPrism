using System.Windows.Controls;
using Recipes_Prism.ViewModels;

namespace Recipes_Prism.Views
{
    /// <summary>
    /// Interaction logic for MountingForm
    /// </summary>
    public partial class MountingForm : UserControl
    {
        public MountingForm()
        {
            InitializeComponent();

            var viewmodel = DataContext as MountingFormViewModel;

            Unloaded += (sender, e) => viewmodel.UnsubscribeEvents();
        }
    }
}
