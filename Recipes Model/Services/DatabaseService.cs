using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recipes_Model.Interfaces;
using Recipes_Model.Database;
using Recipes_Model.Entities;

namespace Recipes_Model.Services
{
    public class DatabaseService : IDatabaseService
    {
        public void Initialize()
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Measure m = null;

                if (session.QueryOver(() => m).Where(() => m.Name == "SZT").RowCount() == 0)
                {
                    session.Save(new Measure { Name = "SZT" });
                    session.Flush();
                }
            }
        }

        public void CheckConnection()
        {
            Hibernate.CheckConnection();
        }
    }
}
