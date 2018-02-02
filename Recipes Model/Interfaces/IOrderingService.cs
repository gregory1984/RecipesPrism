using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recipes_Model.DTO;
using Recipes_Model.DTO.Searching;

namespace Recipes_Model.Interfaces
{
    public interface IOrderingService
    {
        IList<OrderDTO> GetOrders(int pageNo, int pageSize, OrderSearchCriteriaDTO criteria = null);
        int GetOrdersCount(int pageNo, int pageSize, OrderSearchCriteriaDTO criteria = null);

        IList<RecipeDTO> GetRecipesOfOrder(int orderId);
        IList<MountDTO> GetComponentsOfOrder(int orderId);

        IList<RecipeDTO> GetUndefinedRecipes(IList<int> excludedProductIds);

        void AddEditOrder(OrderDTO order, IList<RecipeDTO> recipesOfOrder);
        void DeleteOrder(int orderId);
    }
}
