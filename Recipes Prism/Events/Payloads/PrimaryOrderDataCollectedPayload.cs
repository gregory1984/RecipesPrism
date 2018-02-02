using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes_Prism.Events.Payloads
{
    public class PrimaryOrderDataCollectedPayload
    {
        public string OrderName { get; set; }
        public string OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Comments { get; set; }
        public decimal ItemCount { get; set; }
        public bool IsValid { get; private set; }

        public PrimaryOrderDataCollectedPayload(string orderName, string orderNo, decimal itemCount, DateTime? orderDate, string comments)
        {
            OrderName = orderName;
            OrderNo = orderNo;
            OrderDate = orderDate;
            Comments = comments;
            ItemCount = itemCount;
            IsValid = !string.IsNullOrWhiteSpace(OrderName) && !string.IsNullOrWhiteSpace(OrderNo) && ItemCount > 0 && OrderDate != null;
        }
    }
}
