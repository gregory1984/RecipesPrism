using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Recipes_Model.Entities;

namespace Recipes_Model.Mappings
{
    public class ComponentMap : ClassMap<Component>
    {
        public ComponentMap()
        {
            Id(x => x.Id).Not.Nullable().Unique();
            Map(x => x.Name).Length(200).Not.Nullable().Unique();
            HasMany(x => x.Mounts).Cascade.All().Inverse();
        }
    }
}
