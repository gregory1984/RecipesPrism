using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Data;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.Unity;
using Recipes_Prism.Events;
using Recipes_Prism.Events.Payloads;
using Recipes_Prism.Helpers;
using Recipes_Model.Interfaces;
using Recipes_Model.DTO;


namespace Recipes_Prism.ViewModels
{
    public class ProductDictionaryViewModel : BindableBase
    {
        private string name = "";
        public string Name
        {
            get => name;
            set
            {
                SetProperty(ref name, value);
                Add.RaiseCanExecuteChanged();
            }
        }

        private IList<ProductViewModel> products;
        private ICollectionView productsView;
        public ICollectionView ProductsView
        {
            get => productsView;
            set => SetProperty(ref productsView, value);
        }

        private ProductViewModel selectedProduct;
        public ProductViewModel SelectedProduct
        {
            get => selectedProduct;
            set
            {
                SetProperty(ref selectedProduct, value);

                if (SelectedProduct != null)
                {
                    var payload = new ProductSelectedPayload { Product = SelectedProduct };
                    eventAggregator.GetEvent<ProductSelectedEvent>().Publish(payload);
                }
            }
        }

        private readonly IEventAggregator eventAggregator;
        private readonly IDictionariesService dictionariesService;
        private readonly IUnityContainer unityContainer;
        private SubscriptionToken ProductsDictionaryUpdatedToken;
        private SubscriptionToken ProductMountedUnmountedToken;

        public ProductDictionaryViewModel(IEventAggregator eventAggregator, IDictionariesService dictionariesService, IUnityContainer unityContainer)
        {
            this.eventAggregator = eventAggregator;
            this.dictionariesService = dictionariesService;
            this.unityContainer = unityContainer;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                SetProducts();

                ProductsDictionaryUpdatedToken = eventAggregator.GetEvent<ProductsDictionaryUpdatedEvent>().Subscribe(() =>
                {
                    SetProducts(dictionaryUpdate: true);
                });

                ProductMountedUnmountedToken = eventAggregator.GetEvent<ProductMountedUnmountedEvent>().Subscribe(payload =>
                {
                    SetProducts(dictionaryUpdate: true, selectedProductId: payload.Product.Id);
                });
            }));
        }

        private DelegateCommand find;
        public DelegateCommand Find { get => find ?? (find = new DelegateCommand(() => ProductsView.Refresh())); }

        private DelegateCommand add;
        public DelegateCommand Add
        {
            get => add ?? (add = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    dictionariesService.AddEditProduct(new ProductDTO { Name = Name });
                    SetProducts(dictionaryUpdate: true);
                    Name = "";
                });
            },

            () => eventAggregator.ExecuteSafetyWithResult<bool>(() => !dictionariesService.ProductExists(Name)) && !string.IsNullOrWhiteSpace(Name)));
        }

        private DelegateCommand edit;
        public DelegateCommand Edit
        {
            get => edit ?? (edit = new DelegateCommand(() =>
            {
                unityContainer.RegisterInstance(UnityNames.SelectedDictionaryForEdition, SelectedProduct as DictionaryBaseViewModel);
                eventAggregator.GetEvent<ShowDictionaryEditWindowEvent>().Publish();
            }));
        }

        private DelegateCommand delete;
        public DelegateCommand Delete
        {
            get => delete ?? (delete = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    dictionariesService.DeleteProduct(SelectedProduct.Id);
                    SetProducts(dictionaryUpdate: true);
                });
            }));
        }

        internal void SetProducts(bool dictionaryUpdate = false, int? selectedProductId = null)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    IList<ProductWithCountsDTO> productsDTOs = dictionariesService.GetProductsWithMountsCheck(dictionaryUpdate);
                    products = new List<ProductViewModel>();
                    foreach (var dto in productsDTOs)
                        products.Add(new ProductViewModel(dto, products.Count + 1));

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ProductsView = CollectionViewSource.GetDefaultView(products);
                        ProductsView.Filter = (object item) => (item as ProductViewModel).Name.ToLower().Contains(Name.ToLower());

                        //  Selected product refresh after mounting / unmounting operation.
                        if (selectedProductId.HasValue)
                        {
                            SelectedProduct = (ProductsView.SourceCollection as IEnumerable<ProductViewModel>)
                                .SingleOrDefault(p => p.Id == selectedProductId.Value);
                        }
                    }));
                });
            });
        }

        public void UnsubscribeEvents()
        {
            eventAggregator.GetEvent<ProductsDictionaryUpdatedEvent>().Unsubscribe(ProductsDictionaryUpdatedToken);
            eventAggregator.GetEvent<ProductMountedUnmountedEvent>().Unsubscribe(ProductMountedUnmountedToken);
        }
    }
}
