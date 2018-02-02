using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Recipes_Model.Entities;

namespace Recipes_Model.Mappings
{
    public class OrderMap : ClassMap<Order>
    {
        public OrderMap()
        {
            Id(x => x.Id).Not.Nullable().Unique();
            Map(x => x.ItemCount).Nullable();
            Map(x => x.Name).Not.Nullable().Length(300);
            Map(x => x.OrderNo).Nullable().Length(45);
            Map(x => x.Date).Not.Nullable();
            Map(x => x.Comments).Nullable().Length(1000);
            HasMany(x => x.Recipes).Cascade.All().Inverse();
        }
    }
}
