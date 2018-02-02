using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes_Model.Entities
{
    public class Mount
    {
        public virtual int Id { get; protected set; }
        public virtual decimal MeasureCount { get; set; }
        public virtual decimal ItemCount { get; set; }
        public virtual Component Component { get; set; }
        public virtual Measure Measure { get; set; }
        public virtual Product Product { get; set; }
    }
}
