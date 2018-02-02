using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes_Model.DTO
{
    public class OrderDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string OrderNo { get; set; }
        public decimal ItemCount { get; set; }
        public DateTime Date { get; set; }
        public string Comments { get; set; }
    }
}
