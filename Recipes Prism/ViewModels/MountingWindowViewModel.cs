using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Recipes_Model.Interfaces;
using Recipes_Prism.Events;
using Recipes_Prism.Events.Payloads;
using Recipes_Prism.Helpers;
using Recipes_Model.DTO;

namespace Recipes_Prism.ViewModels
{
    public class MountingWindowViewModel : BindableBase
    {
        private string mountButtonLabel = "ZMONTUJ";
        public string MountButtonLabel
        {
            get => mountButtonLabel;
            set => SetProperty(ref mountButtonLabel, value);
        }

        private string unmountButtonLabel = "ROZMONTUJ";
        public string UnmountButtonLabel
        {
            get => unmountButtonLabel;
            set => SetProperty(ref unmountButtonLabel, value);
        }

        private MountsOfProductCollectedPayload mountPayload;

        private readonly IEventAggregator eventAggregator;
        private readonly IMountingService mountingService;
        private SubscriptionToken ShowDictionaryEditWindowToken;
        private SubscriptionToken ProductSelectedToken;
        private SubscriptionToken MountsOfProductCollected;

        public MountingWindowViewModel(IEventAggregator eventAggregator, IMountingService mountingService)
        {
            this.eventAggregator = eventAggregator;
            this.mountingService = mountingService;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                ShowDictionaryEditWindowToken = eventAggregator.GetEvent<ShowDictionaryEditWindowEvent>().Subscribe(() => ShowDictionaryEditWindowAction?.Invoke());
                ProductSelectedToken = eventAggregator.GetEvent<ProductSelectedEvent>().Subscribe(payload =>
                {
                    MountButtonLabel = "ZMONTUJ " + payload.Product.Name;
                    UnmountButtonLabel = "ROZMONTUJ " + payload.Product.Name;
                });

                MountsOfProductCollected = eventAggregator.GetEvent<MountsOfProductCollectedEvent>().Subscribe(payload =>
                {
                    mountPayload = payload;
                    Mount.RaiseCanExecuteChanged();
                });
            }));
        }

        private DelegateCommand mount;
        public DelegateCommand Mount
        {
            get => mount ?? (mount = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    var selectedProductId = mountPayload.SelectedProduct.Id;
                    var mountsOfProduct = mountPayload.Mounts;

                    var mountsOfProductDTO = new List<MountDTO>();
                    foreach (var vm in mountsOfProduct)
                    {
                        mountsOfProductDTO.Add(new MountDTO
                        {
                            ComponentId = vm.ComponentId,
                            ComponentName = vm.ComponentName,
                            MeasureId = vm.SelectedMeasure.Id,
                            MeasureName = vm.SelectedMeasure.Name,
                            ItemCount = vm.ItemCount,
                            MeasureCount = vm.MeasureCount
                        });
                    }

                    mountingService.MountSelectedProduct(selectedProductId, mountsOfProductDTO);
                    SelectedProductMountedAction?.Invoke(mountPayload.SelectedProduct.Name);

                    var payload = new ProductMountedUnmountedPayload { Product = mountPayload.SelectedProduct };
                    eventAggregator.GetEvent<ProductMountedUnmountedEvent>().Publish(payload);
                });
            },
            () => mountPayload != null ? mountPayload.IsMountable : false));
        }

        private DelegateCommand unmount;
        public DelegateCommand Unmount
        {
            get => unmount ?? (unmount = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    if ((bool)SelectedProductUnmountQuestionAction?.Invoke(mountPayload.SelectedProduct.Name))
                    {
                        mountingService.UnmountSelectedProduct(mountPayload.SelectedProduct.Id);
                        SelectedProductUnmountedAction?.Invoke(mountPayload.SelectedProduct.Name);

                        var payload = new ProductMountedUnmountedPayload { Product = mountPayload.SelectedProduct };
                        eventAggregator.GetEvent<ProductMountedUnmountedEvent>().Publish(payload);
                    }
                });
            }));
        }

        private DelegateCommand closeWindow;
        public DelegateCommand CloseWindow { get => closeWindow ?? (closeWindow = new DelegateCommand(() => CloseWindowAction?.Invoke())); }

        public void UnsubscribePrismEvents()
        {
            eventAggregator.GetEvent<ShowDictionaryEditWindowEvent>().Unsubscribe(ShowDictionaryEditWindowToken);
            eventAggregator.GetEvent<MountsOfProductCollectedEvent>().Unsubscribe(MountsOfProductCollected);
            eventAggregator.GetEvent<ProductSelectedEvent>().Unsubscribe(ProductSelectedToken);
        }

        public Action<Exception> ExceptionOccuredAction { get; set; }
        public Action ShowDictionaryEditWindowAction { get; set; }
        public Action CloseWindowAction { get; set; }
        public Func<string, bool> SelectedProductUnmountQuestionAction { get; set; }
        public Action<string> SelectedProductMountedAction { get; set; }
        public Action<string> SelectedProductUnmountedAction { get; set; }
    }
}
