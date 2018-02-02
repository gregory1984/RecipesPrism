using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Transform;
using NHibernate.Criterion;
using NHibernate;
using Recipes_Model.Interfaces;
using Recipes_Model.DTO;
using Recipes_Model.Entities;
using Recipes_Model.Database;

namespace Recipes_Model.Services
{
    public class MountingService : IMountingService
    {
        public IList<MountDTO> GetSelectedProductMounts(int productId)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Component c = null;
                Mount m = null;
                Product p = null;
                Measure me = null;
                MountDTO dto = null;

                var mounts = session.QueryOver(() => c)
                    .Left.JoinAlias(() => c.Mounts, () => m)
                    .Left.JoinAlias(() => m.Product, () => p)
                    .Left.JoinAlias(() => m.Measure, () => me)
                    .SelectList(l => l
                        .Select(Projections.Group((() => c.Id)).WithAlias(() => dto.ComponentId))
                        .Select(Projections.Group((() => c.Name)).WithAlias(() => dto.ComponentName))
                        .Select(Projections.Max(Projections.Conditional(Restrictions.Eq("p.Id", productId), Projections.Constant(true), Projections.Constant(false)))
                            .WithAlias(() => dto.IsComponentMounted))
                        .Select(Projections.Max(Projections.Conditional(Restrictions.Eq("p.Id", productId), Projections.Property("me.Id"), Projections.Constant(1))))
                            .WithAlias(() => dto.MeasureId)
                        .Select(Projections.Max(Projections.Conditional(Restrictions.Eq("p.Id", productId), Projections.Property("m.MeasureCount"), Projections.Constant(0M, NHibernateUtil.Decimal))))
                            .WithAlias(() => dto.MeasureCount)
                        .Select(Projections.Max(Projections.Conditional(Restrictions.Eq("p.Id", productId), Projections.Property("m.ItemCount"), Projections.Constant(0M, NHibernateUtil.Decimal))))
                            .WithAlias(() => dto.ItemCount))
                    .OrderByAlias(() => dto.IsComponentMounted).Desc
                    .OrderByAlias(() => dto.ComponentName).Asc
                    .TransformUsing(Transformers.AliasToBean<MountDTO>())
                    .List<MountDTO>();

                return mounts;

                #region Upper Sql example for 965 ProductId
                /*
                    SELECT 
                       c.Id AS ComponentId,
                       c.Name AS ComponentName,
                       MAX(CASE WHEN p.id = 965 THEN p.Id ELSE NULL END) AS ProductId,
                       MAX(CASE WHEN p.id = 965 THEN me.Id ELSE NULL END) AS MeasureId,
                       MAX(CASE WHEN p.id = 965 THEN mnt.MeasureCount ELSE NULL END) AS MountMeasureCount,
                       MAX(CASE WHEN p.id = 965 THEN mnt.ItemCount ELSE NULL END) AS MountItemCount
                    FROM `Component` c
                    LEFT JOIN `Mount` mnt ON c.Id = mnt.Component_id
                    LEFT JOIN `Measure` me ON mnt.Measure_id = me.Id
                    LEFT JOIN `Product` p ON mnt.Product_id = p.Id
                    GROUP BY c.Id
                    ORDER BY ProductId DESC, ComponentName ASC;
                */
                #endregion
            }
        }

        public void MountSelectedProduct(int productId, IList<MountDTO> mountsOfProduct)
        {
            UnmountSelectedProduct(productId);
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Product p = null;

                var selectedProduct = session
                    .QueryOver(() => p)
                    .Where(() => p.Id == productId)
                    .SingleOrDefault();

                foreach (var dto in mountsOfProduct)
                {
                    Component c = null;
                    Measure m = null;

                    var mount = new Mount
                    {
                        Product = selectedProduct,
                        Component = session.QueryOver(() => c).Where(() => c.Id == dto.ComponentId).SingleOrDefault(),
                        Measure = session.QueryOver(() => m).Where(() => m.Id == dto.MeasureId).SingleOrDefault(),
                        ItemCount = dto.ItemCount,
                        MeasureCount = dto.MeasureCount
                    };
                    session.Clear();
                    session.SaveOrUpdate(mount);
                    session.Flush();
                }
            }
        }

        public void UnmountSelectedProduct(int productId)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Mount m = null;
                Product p = null;

                var mountsOfProduct = session.QueryOver(() => m)
                    .JoinAlias(() => m.Product, () => p)
                    .Where(() => p.Id == productId)
                    .List();

                foreach (var mnt in mountsOfProduct)
                {
                    session.Delete(mnt);
                }
                session.Flush();
            }
        }
    }
}
