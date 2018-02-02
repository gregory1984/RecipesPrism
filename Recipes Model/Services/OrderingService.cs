using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Transform;
using NHibernate.Criterion;
using NHibernate.Dialect.Function;
using NHibernate.Type;
using NHibernate;
using Recipes_Model.Entities;
using Recipes_Model.ExtMethods;
using Recipes_Model.Database;
using Recipes_Model.Interfaces;
using Recipes_Model.DTO;
using Recipes_Model.DTO.Searching;


namespace Recipes_Model.Services
{
    public class OrderingService : IOrderingService
    {
        public IList<OrderDTO> GetOrders(int pageNo, int pageSize, OrderSearchCriteriaDTO criteria = null)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Recipes_Model.Entities.Order o = null;
                OrderDTO dto = null;

                var orders = session.QueryOver(() => o)
                    .SelectList(list => list
                        .Select(() => o.Id).WithAlias(() => dto.Id)
                        .Select(() => o.Name).WithAlias(() => dto.Name)
                        .Select(() => o.OrderNo).WithAlias(() => dto.OrderNo)
                        .Select(() => o.Comments).WithAlias(() => dto.Comments)
                        .Select(() => o.ItemCount).WithAlias(() => dto.ItemCount)
                        .Select(() => o.Date).WithAlias(() => dto.Date));

                if (criteria != null)
                {
                    if (criteria.OrderName.IsNotNull()) orders.Where(Restrictions.InsensitiveLike(nameof(o.Name), criteria.OrderName, MatchMode.Anywhere));
                    if (criteria.OrderNo.IsNotNull()) orders.Where(Restrictions.InsensitiveLike(nameof(o.OrderNo), criteria.OrderNo, MatchMode.Anywhere));
                    if (criteria.OrderDate.IsNotNull()) orders.Where(() => o.Date.Date == criteria.OrderDate.Value.Date);
                    if (criteria.ComponentName.IsNotNull())
                    {
                        Recipe r = null;
                        Product p = null;
                        Mount m = null;
                        Component c = null;

                        orders
                            .JoinAlias(() => o.Recipes, () => r)
                            .JoinAlias(() => r.Product, () => p)
                            .JoinAlias(() => p.Mounts, () => m)
                            .JoinAlias(() => m.Component, () => c)
                            .Where(Restrictions.InsensitiveLike("c.Name", criteria.ComponentName, MatchMode.Anywhere));
                    }
                }

                return orders
                    .OrderBy(() => o.Name).Asc
                    .TransformUsing(Transformers.AliasToBean<OrderDTO>())
                    .Skip((pageNo - 1) * pageSize).Take(pageSize)
                    .List<OrderDTO>();
            }
        }

        public int GetOrdersCount(int pageNo, int pageSize, OrderSearchCriteriaDTO criteria = null)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Recipes_Model.Entities.Order o = null;
                var count = session.QueryOver(() => o);

                if (criteria != null)
                {
                    if (criteria.OrderName.IsNotNull()) count.Where(Restrictions.InsensitiveLike(nameof(o.Name), criteria.OrderName, MatchMode.Anywhere));
                    if (criteria.OrderNo.IsNotNull()) count.Where(Restrictions.InsensitiveLike(nameof(o.OrderNo), criteria.OrderNo, MatchMode.Anywhere));
                    if (criteria.OrderDate.IsNotNull()) count.Where(() => o.Date.Date == criteria.OrderDate.Value.Date);
                    if (criteria.ComponentName.IsNotNull())
                    {
                        Recipe r = null;
                        Product p = null;
                        Mount m = null;
                        Component c = null;

                        count
                            .JoinAlias(() => o.Recipes, () => r)
                            .JoinAlias(() => r.Product, () => p)
                            .JoinAlias(() => p.Mounts, () => m)
                            .JoinAlias(() => m.Component, () => c)
                            .Where(Restrictions.InsensitiveLike("c.Name", criteria.ComponentName, MatchMode.Anywhere));
                    }
                }

                var result = Convert.ToInt32(Math.Ceiling((1.0 * count.RowCount()) / pageSize));
                return result == 0 ? 1 : result;
            }
        }

        public IList<RecipeDTO> GetRecipesOfOrder(int orderId)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Recipe r = null;
                Recipes_Model.Entities.Order o = null;
                Product p = null;
                Measure m = null;

                RecipeDTO dto = null;

                var recipes = session.QueryOver(() => r)
                    .SelectList(list => list
                        .Select(() => r.Id).WithAlias(() => dto.Id)
                        .Select(() => p.Id).WithAlias(() => dto.ProductId)
                        .Select(() => p.Name).WithAlias(() => dto.ProductName)
                        .Select(() => m.Id).WithAlias(() => dto.MeasureId)
                        .Select(() => m.Name).WithAlias(() => dto.MeasureName)
                        .Select(() => r.MeasureCount).WithAlias(() => dto.MeasureCount))
                    .JoinAlias(() => r.Order, () => o)
                    .JoinAlias(() => r.Product, () => p)
                    .JoinAlias(() => r.Measure, () => m)
                    .Where(() => o.Id == orderId)
                    .OrderBy(() => p.Name).Asc
                    .TransformUsing(Transformers.AliasToBean<RecipeDTO>())
                    .List<RecipeDTO>();

                return recipes;
            }
        }

        public IList<RecipeDTO> GetUndefinedRecipes(IList<int> excludedProductIds)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Product p = null;
                Measure m = null;

                RecipeDTO dto = null;

                var defaultItemCountMeasureName = "SZT";
                var dedicatedMeasure = session.QueryOver(() => m)
                    .Where(() => m.Name == defaultItemCountMeasureName)
                    .SingleOrDefault();

                var undefined = session.QueryOver(() => p)
                    .SelectList(list => list
                        .Select(() => p.Id).WithAlias(() => dto.ProductId)
                        .Select(() => p.Name).WithAlias(() => dto.ProductName)
                        .Select(() => dedicatedMeasure.Id).WithAlias(() => dto.MeasureId)
                        .Select(() => dedicatedMeasure.Name).WithAlias(() => dto.MeasureName)
                        .Select(() => decimal.Zero).WithAlias(() => dto.MeasureCount))
                    .Where(Restrictions.Not(Restrictions.In(nameof(p.Id), excludedProductIds.ToList())))
                    .OrderBy(() => p.Name).Asc
                    .TransformUsing(Transformers.AliasToBean<RecipeDTO>())
                    .List<RecipeDTO>();

                return undefined;
            }
        }

        public IList<MountDTO> GetComponentsOfOrder(int orderId)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Recipes_Model.Entities.Order o = null;
                Recipe r = null;
                Product p = null;
                Mount m = null;
                Measure me = null;
                Component c = null;

                MountDTO dto = null;

                var mounts = session.QueryOver(() => o)
                    .JoinAlias(() => o.Recipes, () => r)
                    .JoinAlias(() => r.Product, () => p)
                    .JoinAlias(() => p.Mounts, () => m)
                    .JoinAlias(() => m.Component, () => c)
                    .JoinAlias(() => m.Measure, () => me)
                    .SelectList(list => list
                        .Select(Projections.Group(() => c.Id)).WithAlias(() => dto.ComponentId)
                        .Select(Projections.Group(() => c.Name)).WithAlias(() => dto.ComponentName)
                        .Select(Projections.Group(() => me.Id)).WithAlias(() => dto.MeasureId)
                        .Select(Projections.Group(() => me.Name)).WithAlias(() => dto.MeasureName)
                        .Select(Projections.Sum(
                            Projections.SqlFunction(
                                new VarArgsSQLFunction("(", "*", ")"), NHibernateUtil.Decimal,
                                new[]
                                {
                                    Projections.Property("m.MeasureCount"), Projections.Property("r.MeasureCount")
                                }
                            ))).WithAlias(() => dto.MeasureCount)
                        .Select(Projections.Sum(
                            Projections.SqlFunction(
                                new VarArgsSQLFunction("(", "*", ")"), NHibernateUtil.Decimal,
                                new[]
                                {
                                    Projections.Property("m.ItemCount"), Projections.Property("r.MeasureCount")
                                }
                            )).WithAlias(() => dto.ItemCount)))
                    .Where(() => o.Id == orderId)
                    .OrderBy(() => o.Name).Asc
                    .TransformUsing(Transformers.AliasToBean<MountDTO>())
                    .List<MountDTO>();

                return mounts;
            }
        }

        public void AddEditOrder(OrderDTO order, IList<RecipeDTO> recipesOfOrder)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Entities.Order o = null;

                var entityOfOrder = session.QueryOver(() => o)
                    .Where(() => o.Id == order.Id)
                    .SingleOrDefault();

                if (entityOfOrder == null)
                    entityOfOrder = new Entities.Order();

                entityOfOrder.Comments = order.Comments;
                entityOfOrder.Date = order.Date;
                entityOfOrder.ItemCount = order.ItemCount;
                entityOfOrder.Name = order.Name;
                entityOfOrder.OrderNo = order.OrderNo;

                session.Clear();
                session.SaveOrUpdate(entityOfOrder);
                session.Flush();

                Recipe rec = null;
                o = null;

                var recipesForDeletion = session.QueryOver(() => rec)
                    .JoinAlias(() => rec.Order, () => o)
                    .Where(() => o.Id == order.Id)
                    .List();

                foreach (var r in recipesForDeletion)
                {
                    session.Delete(r);
                    session.Flush();
                }

                foreach (var r in recipesOfOrder)
                {
                    Measure m = null;
                    Product p = null;

                    var entityOfRecipe = new Recipe
                    {
                        Measure = session.QueryOver(() => m).Where(() => m.Id == r.MeasureId).SingleOrDefault(),
                        Product = session.QueryOver(() => p).Where(() => p.Id == r.ProductId).SingleOrDefault(),
                        MeasureCount = r.MeasureCount,
                        Order = entityOfOrder
                    };

                    session.Clear();
                    session.SaveOrUpdate(entityOfRecipe);
                    session.Flush();
                }
            }
        }

        public void DeleteOrder(int orderId)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Entities.Order o = null;

                var entityOfOrder = session.QueryOver(() => o)
                    .Where(() => o.Id == orderId)
                    .SingleOrDefault();

                session.Clear();
                session.Delete(entityOfOrder);
                session.Flush();
            }
        }
    }
}
