using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes_Model.DTO
{
    public class MountDTO
    {
        public int ComponentId { get; set; }
        public string ComponentName { get; set; }
        public bool IsComponentMounted { get; set; }
        public int MeasureId { get; set; }
        public string MeasureName { get; set; }
        public decimal MeasureCount { get; set; }
        public decimal ItemCount { get; set; }
    }
}
