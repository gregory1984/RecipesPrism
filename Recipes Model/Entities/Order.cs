using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes_Model.Entities
{
    public class Order
    {
        public virtual int Id { get; protected set; }
        public virtual string Name { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string OrderNo { get; set; }
        public virtual decimal? ItemCount { get; set; }
        public virtual string Comments { get; set; }
        public virtual IList<Recipe> Recipes { get; set; }

        public Order()
        {
            Recipes = new List<Recipe>();
        }
    }
}
