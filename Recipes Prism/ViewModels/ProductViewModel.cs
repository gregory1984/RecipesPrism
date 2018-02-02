using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Recipes_Model.DTO;

namespace Recipes_Prism.ViewModels
{
    public class ProductViewModel : DictionaryBaseViewModel
    {
        private string color = "Transparent";
        public string Color
        {
            get => color;
            set => SetProperty(ref color, value);
        }

        public ProductViewModel(ProductWithCountsDTO dto, int no, bool withColors = false)
        {
            Id = dto.Id;
            Name = dto.Name;
            CanBeDeleted = dto.MountsCount == 0 && dto.RecipesCount == 0;
            No = no;

            if (withColors)
                Color = !CanBeDeleted ? "#d3f1fd" : "Transparent";
        }
    }
}
