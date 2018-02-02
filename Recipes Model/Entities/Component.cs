using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes_Model.Entities
{
    public class Component
    {
        public virtual int Id { get; protected set; }
        public virtual string Name { get; set; }
        public virtual IList<Mount> Mounts { get; set; }

        public Component()
        {
            Mounts = new List<Mount>();
        }
    }
}
