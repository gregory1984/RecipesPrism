using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recipes_Prism.ViewModels;

namespace Recipes_Prism.Events.Payloads
{
    public class RecipesOfOrderCollectedPayload
    {
        public IList<RecipeViewModel> Recipes { get; set; }
        public bool IsValid { get; set; }
    }
}
