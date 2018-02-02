using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes_Model.DTO
{
    public class RecipeDTO
    {
        public int? Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal MeasureCount { get; set; }
        public int MeasureId { get; set; }
        public string MeasureName { get; set; }

        public RecipeDTO() { }

        public RecipeDTO(int? id, int productId, string productName, decimal measureCount, int measureId, string measureName)
        {
            Id = id;
            ProductId = productId;
            ProductName = productName;
            MeasureCount = measureCount;
            MeasureId = measureId;
            MeasureName = measureName;
        }
    }
}
