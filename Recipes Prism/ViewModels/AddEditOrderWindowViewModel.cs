using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Data;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.Unity;
using Recipes_Prism.Events;
using Recipes_Prism.Events.Payloads;
using Recipes_Prism.Events.Pagination;
using Recipes_Prism.Enums;
using Recipes_Prism.Helpers;
using Recipes_Model.Interfaces;
using Recipes_Model.DTO;
using Recipes_Model.DTO.Searching;

namespace Recipes_Prism.ViewModels
{
    public class AddEditOrderWindowViewModel : BindableBase
    {
        private string windowDescription = "";
        public string WindowDescription
        {
            get => windowDescription;
            set => SetProperty(ref windowDescription, value);
        }

        private string buttonImage = "";
        public string ButtonImage
        {
            get => buttonImage;
            set => SetProperty(ref buttonImage, value);
        }

        private string buttonText = "";
        public string ButtonText
        {
            get => buttonText;
            set => SetProperty(ref buttonText, value);
        }

        private readonly IEventAggregator eventAggregator;
        private readonly IOrderingService orderingService;
        private readonly IUnityContainer unityContainer;

        private SubscriptionToken PrimaryOrderDataCollectedToken;
        private SubscriptionToken RecipesOfOrderCollectedToken;

        private WindowMode windowMode;
        private OrderViewModel selectedOrder;
        private PrimaryOrderDataCollectedPayload primaryDataOfOrder;
        private RecipesOfOrderCollectedPayload recipesOfOrder;

        public AddEditOrderWindowViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IOrderingService orderingService)
        {
            this.eventAggregator = eventAggregator;
            this.orderingService = orderingService;
            this.unityContainer = unityContainer;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    windowMode = unityContainer.Resolve<WindowMode>(UnityNames.AddEditOrderWindowMode);
                    if (windowMode == WindowMode.Edition)
                    {
                        selectedOrder = unityContainer.Resolve<OrderViewModel>(UnityNames.SelectedOrderForEdition);
                        WindowDescription = "EDYCJA ZLECENIA: [ " + selectedOrder.Name + " ]";
                        ButtonText = "ZMIEŃ";
                        ButtonImage = "/Recipes Prism;component/Images/Icons/Edit.png";
                    }
                    else
                    {
                        WindowDescription = "NOWE ZLECENIE";
                        ButtonText = "DODAJ";
                        ButtonImage = "/Recipes Prism;component/Images/Icons/Add.png";
                    }

                    PrimaryOrderDataCollectedToken = eventAggregator.GetEvent<PrimaryOrderDataCollectedEvent>().Subscribe(payload =>
                    {
                        primaryDataOfOrder = payload;
                        AddEditOrder.RaiseCanExecuteChanged();
                    });

                    RecipesOfOrderCollectedToken = eventAggregator.GetEvent<RecipesOfOrderCollectedEvent>().Subscribe(payload =>
                    {
                        recipesOfOrder = payload;
                        AddEditOrder.RaiseCanExecuteChanged();
                    });
                });
            }));
        }

        private DelegateCommand addEditOrder;
        public DelegateCommand AddEditOrder
        {
            get => addEditOrder ?? (addEditOrder = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    OrderDTO orderDTO = new OrderDTO
                    {
                        Comments = primaryDataOfOrder.Comments,
                        Date = primaryDataOfOrder.OrderDate.Value,
                        ItemCount = primaryDataOfOrder.ItemCount,
                        Name = primaryDataOfOrder.OrderName,
                        OrderNo = primaryDataOfOrder.OrderNo
                    };

                    IList<RecipeDTO> recipesOfOrderDTOs = new List<RecipeDTO>();
                    foreach (var r in recipesOfOrder.Recipes)
                        recipesOfOrderDTOs.Add(new RecipeDTO(r.Id, r.ProductId, r.ProductName, r.MeasureCount, r.MeasureId, r.MeasureName));

                    if (windowMode == WindowMode.Edition)
                    {
                        if ((bool)EditOrderQuestionAction?.Invoke(primaryDataOfOrder.OrderName))
                        {
                            orderDTO.Id = selectedOrder.Id;
                            orderingService.AddEditOrder(orderDTO, recipesOfOrderDTOs);
                            OrderAddedEditedAction?.Invoke(primaryDataOfOrder.OrderName, windowMode);
                            eventAggregator.GetEvent<OrderAddedEditedEvent>().Publish();
                        }
                    }
                    else
                    {
                        orderingService.AddEditOrder(orderDTO, recipesOfOrderDTOs);
                        OrderAddedEditedAction?.Invoke(primaryDataOfOrder.OrderName, windowMode);
                        eventAggregator.GetEvent<OrderAddedEditedEvent>().Publish();
                    }
                });
            },

            () => (primaryDataOfOrder != null ? primaryDataOfOrder.IsValid : false) && recipesOfOrder != null ? recipesOfOrder.IsValid : false));
        }

        private DelegateCommand closeWindow;
        public DelegateCommand CloseWindow
        {
            get => closeWindow ?? (closeWindow = new DelegateCommand(() => CloseWindowAction?.Invoke()));
        }

        public void UnsubscribeEvents()
        {
            eventAggregator.GetEvent<PrimaryOrderDataCollectedEvent>().Unsubscribe(PrimaryOrderDataCollectedToken);
        }

        public Func<string, bool> EditOrderQuestionAction { get; set; }
        public Action<string, WindowMode> OrderAddedEditedAction { get; set; }
        public Action CloseWindowAction { get; set; }
    }
}
