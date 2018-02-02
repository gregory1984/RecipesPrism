using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recipes_Model.DTO;

namespace Recipes_Prism.ViewModels
{
    public class MeasureViewModel : DictionaryBaseViewModel
    {
        public MeasureViewModel(MeasureWithCountsDTO dto, int no)
        {
            Id = dto.Id;
            Name = dto.Name;
            CanBeDeleted = dto.MountsCount == 0 && dto.RecipesCount == 0;
            No = no;
        }

        public MeasureViewModel(MeasureWithCountsDTO dto)
        {
            Id = dto.Id;
            Name = dto.Name;
        }

        public MeasureViewModel(MeasureViewModel vm)
        {
            Id = vm.Id;
            Name = vm.Name;
            CanBeDeleted = vm.CanBeDeleted;
            No = vm.No;
            WasEditedFromMountsForm = vm.WasEditedFromMountsForm;
        }
    }
}
