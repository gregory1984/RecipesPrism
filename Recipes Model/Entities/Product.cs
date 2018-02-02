using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes_Model.Entities
{
    public class Product
    {
        public virtual int Id { get; protected set; }
        public virtual string Name { get; set; }
        public virtual IList<Mount> Mounts { get; set; }
        public virtual IList<Recipe> Recipes { get; set; }

        public Product()
        {
            Mounts = new List<Mount>();
            Recipes = new List<Recipe>();
        }
    }
}
