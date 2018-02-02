using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recipes_Model.DTO;

namespace Recipes_Model.Interfaces
{
    public interface IDictionariesService
    {
        bool ProductExists(string productName);
        void DeleteProduct(int id);
        void AddEditProduct(ProductDTO product);
        IList<ProductWithCountsDTO> GetProductsWithMountsCheck(bool update = false);

        bool ComponentExists(string componentName);
        void DeleteComponent(int id);
        void AddEditComponent(ComponentDTO component);
        IList<ComponentWithCountsDTO> GetComponentsWithMountsCheck(bool update = false);
        void ForceRefreshComponentsDictionary();

        bool MeasureExists(string measureName);
        void DeleteMeasure(int id);
        void AddEditMeasure(MeasureDTO component);
        IList<MeasureWithCountsDTO> GetMeasuresWithMountsCheck(bool update = false);
    }
}
