using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes_Model.DTO.Searching
{
    public class OrderSearchCriteriaDTO
    {
        public string OrderName { get; set; }
        public string OrderNo { get; set; }
        public string ComponentName { get; set; }
        public DateTime? OrderDate { get; set; }

        public void Clear()
        {
            OrderName = OrderNo = ComponentName = "";
            OrderDate = null;
        }
    }
}
