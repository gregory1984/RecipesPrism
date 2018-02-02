using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Data;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Practices.Unity;
using Recipes_Prism.Events;
using Recipes_Prism.Events.Payloads;
using Recipes_Prism.Helpers;
using Recipes_Model.DTO;
using Recipes_Model.Interfaces;

namespace Recipes_Prism.ViewModels
{
    public class MountingFormViewModel : BindableBase
    {
        private string name = "";
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string selectedProductName = "";
        public string SelectedProductName
        {
            get => selectedProductName;
            set => SetProperty(ref selectedProductName, value);
        }

        private IList<MountViewModel> mounts;
        private ICollectionView mountsView;
        public ICollectionView MountsView
        {
            get => mountsView;
            set => SetProperty(ref mountsView, value);
        }

        private MountViewModel selectedMount;
        public MountViewModel SelectedMount
        {
            get => selectedMount;
            set => SetProperty(ref selectedMount, value);
        }

        private ProductViewModel selectedProduct;

        private readonly IEventAggregator eventAggregator;
        private readonly IMountingService mountingService;
        private readonly IUnityContainer unityContainer;
        private readonly IDictionariesService dictionariesService;
        private SubscriptionToken ProductSelectedToken;
        private SubscriptionToken ComponentOfSelectedMountUpdatedToken;

        public MountingFormViewModel(IEventAggregator eventAggregator, IMountingService mountingService, IUnityContainer unityContainer, IDictionariesService dictionariesService)
        {
            this.eventAggregator = eventAggregator;
            this.mountingService = mountingService;
            this.unityContainer = unityContainer;
            this.dictionariesService = dictionariesService;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                ProductSelectedToken = eventAggregator.GetEvent<ProductSelectedEvent>().Subscribe(payload =>
                {
                    selectedProduct = payload.Product;
                    SelectedProductName = "Składniki produktu: " + payload.Product.Name;
                    Name = "";
                    SetMounts(payload.Product.Id);
                });

                ComponentOfSelectedMountUpdatedToken = eventAggregator.GetEvent<ComponentOfSelectedMountUpdatedEvent>().Subscribe(payload =>
                {
                    SelectedMount.ComponentName = payload.Component.Name;
                    MountsView.Refresh();
                });
            }));
        }

        private DelegateCommand findComponent;
        public DelegateCommand FindComponent { get => findComponent ?? (findComponent = new DelegateCommand(() => MountsView.Refresh())); }

        private DelegateCommand addComponent;
        public DelegateCommand AddComponent
        {
            get
            {
                return addComponent ?? (addComponent = new DelegateCommand(() =>
                {
                    eventAggregator.ExecuteSafety(() =>
                    {
                        dictionariesService.AddEditComponent(new ComponentDTO { Name = this.Name });
                        dictionariesService.ForceRefreshComponentsDictionary();
                        MountsView.Refresh();
                    });
                },

                () => eventAggregator.ExecuteSafetyWithResult<bool>(() => !dictionariesService.ComponentExists(Name))));
            }
        }

        private DelegateCommand editComponent;
        public DelegateCommand EditComponent
        {
            get => editComponent ?? (editComponent = new DelegateCommand(() =>
            {
                unityContainer.RegisterInstance(UnityNames.SelectedDictionaryForEdition, new ComponentViewModel(SelectedMount) as DictionaryBaseViewModel);
                eventAggregator.GetEvent<ShowDictionaryEditWindowEvent>().Publish();
            }));
        }

        private DelegateCommand deleteComponent;
        public DelegateCommand DeleteComponent
        {
            get => deleteComponent ?? (deleteComponent = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    dictionariesService.DeleteComponent(SelectedMount.ComponentId);
                    dictionariesService.ForceRefreshComponentsDictionary();

                    mounts.Remove(mounts.SingleOrDefault(m => m.ComponentId == SelectedMount.ComponentId));
                    MountsView.Refresh();
                });
            }));
        }

        private void SetMounts(int productId)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    IList<MountDTO> mountsDTOs = mountingService.GetSelectedProductMounts(productId);
                    mounts = new List<MountViewModel>();
                    foreach (var dto in mountsDTOs)
                    {
                        var mount_vm = new MountViewModel(dictionariesService, dto, mounts.Count + 1);
                        mount_vm.RequiredDataCollectedAction += () =>
                        {
                            var payload = new MountsOfProductCollectedPayload
                            {
                                SelectedProduct = selectedProduct,
                                Mounts = mounts.Where(m => m.IsChecked || m.ItemCount != decimal.Zero).ToList(),
                                IsMountable = mounts.Where(m => m.IsChecked || m.ItemCount != decimal.Zero).Any() &&
                                              mounts.Where(m => m.IsChecked).Count() == mounts.Where(m => m.ItemCount != decimal.Zero).Count()
                            };
                            eventAggregator.GetEvent<MountsOfProductCollectedEvent>().Publish(payload);
                        };

                        mount_vm.RequiredDataCollectedAction?.Invoke();
                        mounts.Add(mount_vm);
                    }

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MountsView = CollectionViewSource.GetDefaultView(mounts);
                        MountsView.Filter = (object item) => (item as MountViewModel).ComponentName.ToLower().Contains(Name.ToLower());
                    }));
                });
            });
        }

        public void UnsubscribeEvents()
        {
            eventAggregator.GetEvent<ProductSelectedEvent>().Unsubscribe(ProductSelectedToken);
            eventAggregator.GetEvent<ComponentsDictionaryUpdatedEvent>().Unsubscribe(ComponentOfSelectedMountUpdatedToken);
        }
    }
}
