using System.Windows.Controls;
using MahApps.Metro.Controls;
using Recipes_Prism.ViewModels;

namespace Recipes_Prism.Views
{
    /// <summary>
    /// Interaction logic for AboutWindow
    /// </summary>
    public partial class AboutWindow : MetroWindow
    {
        public AboutWindow()
        {
            InitializeComponent();

            var viewmodel = DataContext as AboutWindowViewModel;
            viewmodel.CloseWindowAction += () => Close();
        }
    }
}
