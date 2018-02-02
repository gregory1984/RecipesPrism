using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes_Prism.Events.Payloads
{
    public class OrderSearchCriteriaChangedPayload
    {
        public string OrderName { get; set; } = "";
        public string OrderNo { get; set; } = "";
        public string ComponentName { get; set; } = "";
        public DateTime? OrderDate { get; set; } = null;

        public bool IsClear()
        {
            return string.IsNullOrWhiteSpace(OrderName)
                && string.IsNullOrWhiteSpace(OrderNo)
                && string.IsNullOrWhiteSpace(ComponentName)
                && OrderDate == null;
        }
    }
}
