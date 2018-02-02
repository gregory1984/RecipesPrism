using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recipes_Model.DTO;

namespace Recipes_Model.Interfaces
{
    public interface IMountingService
    {
        IList<MountDTO> GetSelectedProductMounts(int productId);
        void UnmountSelectedProduct(int productId);
        void MountSelectedProduct(int productId, IList<MountDTO> mountsOfProduct);
    }
}
