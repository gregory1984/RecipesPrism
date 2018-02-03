using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes_Model.Interfaces
{
    public interface IDatabaseService
    {
        void Initialize();
        void CheckConnection();
    }
}
