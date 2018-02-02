using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Recipes_Model.Entities;

namespace Recipes_Model.Mappings
{
    public class RecipeMap : ClassMap<Recipe>
    {
        public RecipeMap()
        {
            Id(x => x.Id).Not.Nullable().Unique();
            Map(x => x.MeasureCount).Not.Nullable();
            References(x => x.Measure);
            References(x => x.Order);
            References(x => x.Product);
        }
    }
}
