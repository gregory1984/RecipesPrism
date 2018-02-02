using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recipes_Model.Interfaces;
using Recipes_Model.Database;

namespace Recipes_Model.Services
{
    public class DatabaseService : IDatabaseService
    {
        public void CheckConnection()
        {
            Hibernate.CheckConnection();
        }
    }
}
