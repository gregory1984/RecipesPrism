using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recipes_Model.DTO;

namespace Recipes_Prism.ViewModels
{
    public class ComponentViewModel : DictionaryBaseViewModel
    {
        public ComponentViewModel(MountViewModel mountViewModel)
        {
            Id = mountViewModel.ComponentId;
            Name = mountViewModel.ComponentName;
            CanBeDeleted = mountViewModel.CanBeDeleted;
            No = mountViewModel.No;
            WasEditedFromMountsForm = true;
        }

        public ComponentViewModel(ComponentWithCountsDTO dto, int no)
        {
            Id = dto.Id;
            Name = dto.Name;
            CanBeDeleted = dto.MountsCount == 0;
            No = no;
        }
    }
}
