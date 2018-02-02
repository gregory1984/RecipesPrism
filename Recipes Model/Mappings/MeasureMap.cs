using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Recipes_Model.Entities;

namespace Recipes_Model.Mappings
{
    public class MeasureMap : ClassMap<Measure>
    {
        public MeasureMap()
        {
            Id(x => x.Id).Not.Nullable().Unique();
            Map(x => x.Name).Not.Nullable().Length(200).Unique();
            HasMany(x => x.Mounts).Cascade.All().Inverse();
            HasMany(x => x.Recipes).Cascade.All().Inverse();
        }
    }
}
