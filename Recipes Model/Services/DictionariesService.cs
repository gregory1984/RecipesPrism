using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Recipes_Model.Interfaces;
using Recipes_Model.Entities;
using Recipes_Model.DTO;
using Recipes_Model.Database;
using NHibernate.Transform;
using NHibernate.Criterion;

namespace Recipes_Model.Services
{
    public class DictionariesService : IDictionariesService
    {
        private IList<ProductWithCountsDTO> products;
        private IList<ComponentWithCountsDTO> components;
        private IList<MeasureWithCountsDTO> measures;

        #region Products
        public bool ProductExists(string productName)
        {
            return GetProductsWithMountsCheck().Any(p => p.Name.ToLower() == productName.ToLower());
        }

        public void DeleteProduct(int id)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Product p = null;

                var product = session.QueryOver(() => p).Where(() => p.Id == id).SingleOrDefault();

                session.Delete(product);
                session.Flush();
            }
        }

        public void AddEditProduct(ProductDTO product)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Product p = null;
                Product entity = null;

                if (product.Id != null)
                    entity = session.QueryOver(() => p).Where(() => p.Id == product.Id.Value).SingleOrDefault();
                else
                    entity = new Product();

                entity.Name = product.Name;

                session.SaveOrUpdate(entity);
                session.Flush();
            }
        }

        public IList<ProductWithCountsDTO> GetProductsWithMountsCheck(bool update = false)
        {
            if (products == null || update)
            {
                using (var session = Hibernate.SessionFactory.OpenSession())
                {
                    Product p = null;
                    Mount m = null;
                    Recipe r = null;
                    ProductWithCountsDTO dto = null;

                    products = session
                        .QueryOver(() => p)
                        .Left.JoinAlias(() => p.Mounts, () => m)
                        .Left.JoinAlias(() => p.Recipes, () => r)
                        .SelectList(l => l
                            .SelectGroup(() => p.Id).WithAlias(() => dto.Id)
                            .SelectGroup(() => p.Name).WithAlias(() => dto.Name)
                            .SelectCount(() => m.Id).WithAlias(() => dto.MountsCount)
                            .SelectCount(() => r.Id).WithAlias(() => dto.RecipesCount))
                        .OrderBy(() => p.Name).Asc
                        .TransformUsing(Transformers.AliasToBean<ProductWithCountsDTO>())
                        .List<ProductWithCountsDTO>();
                }
            }
            return products;
        }
        #endregion Products

        #region Components
        public bool ComponentExists(string componentName)
        {
            return GetComponentsWithMountsCheck().Any(p => p.Name.ToLower() == componentName.ToLower());
        }

        public void DeleteComponent(int id)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Component c = null;

                var component = session.QueryOver(() => c).Where(() => c.Id == id).SingleOrDefault();

                session.Delete(component);
                session.Flush();
            }
        }

        public void AddEditComponent(ComponentDTO component)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Component c = null;
                Component entity = null;

                if (component.Id != null)
                    entity = session.QueryOver(() => c).Where(() => c.Id == component.Id.Value).SingleOrDefault();
                else
                    entity = new Component();

                entity.Name = component.Name;

                session.SaveOrUpdate(entity);
                session.Flush();
            }
        }

        public IList<ComponentWithCountsDTO> GetComponentsWithMountsCheck(bool update = false)
        {
            if (components == null || update)
            {
                ForceRefreshComponentsDictionary();
            }
            return components;
        }

        public void ForceRefreshComponentsDictionary()
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Component c = null;
                Mount m = null;
                ComponentWithCountsDTO dto = null;

                components = session
                    .QueryOver(() => c)
                    .Left.JoinAlias(() => c.Mounts, () => m)
                    .SelectList(l => l
                        .SelectGroup(() => c.Id).WithAlias(() => dto.Id)
                        .SelectGroup(() => c.Name).WithAlias(() => dto.Name)
                        .SelectCount(() => m.Id).WithAlias(() => dto.MountsCount))
                    .OrderBy(() => c.Name).Asc
                    .TransformUsing(Transformers.AliasToBean<ComponentWithCountsDTO>())
                    .List<ComponentWithCountsDTO>();
            }
        }
        #endregion Components

        #region Measures
        public bool MeasureExists(string measureName)
        {
            return GetMeasuresWithMountsCheck().Any(m => m.Name.ToLower() == measureName.ToLower());
        }

        public void DeleteMeasure(int id)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Measure m = null;

                var measure = session.QueryOver(() => m).Where(() => m.Id == id).SingleOrDefault();

                session.Delete(measure);
                session.Flush();
            }
        }

        public void AddEditMeasure(MeasureDTO measure)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Measure m = null;
                Measure entity = null;

                if (measure.Id != null)
                    entity = session.QueryOver(() => m).Where(() => m.Id == measure.Id.Value).SingleOrDefault();
                else
                    entity = new Measure();

                entity.Name = measure.Name;

                session.SaveOrUpdate(entity);
                session.Flush();
            }
        }

        public IList<MeasureWithCountsDTO> GetMeasuresWithMountsCheck(bool update = false)
        {
            if (measures == null || update)
            {
                using (var session = Hibernate.SessionFactory.OpenSession())
                {
                    Measure m = null;
                    Mount mo = null;
                    Recipe r = null;
                    MeasureWithCountsDTO dto = null;

                    measures = session
                        .QueryOver(() => m)
                        .Left.JoinAlias(() => m.Mounts, () => mo)
                        .Left.JoinAlias(() => m.Recipes, () => r)
                        .SelectList(l => l
                            .SelectGroup(() => m.Id).WithAlias(() => dto.Id)
                            .SelectGroup(() => m.Name).WithAlias(() => dto.Name)
                            .SelectCount(() => mo.Id).WithAlias(() => dto.MountsCount)
                            .SelectCount(() => r.Id).WithAlias(() => dto.RecipesCount))
                        .OrderBy(() => m.Name).Asc
                        .TransformUsing(Transformers.AliasToBean<MeasureWithCountsDTO>())
                        .List<MeasureWithCountsDTO>();
                }
            }
            return measures;
        }
        #endregion Measures
    }
}
