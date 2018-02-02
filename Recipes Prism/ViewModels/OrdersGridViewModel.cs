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
using Recipes_Prism.Helpers;
using Recipes_Prism.Enums;
using Recipes_Model.Interfaces;
using Recipes_Model.DTO;
using Recipes_Model.DTO.Searching;

namespace Recipes_Prism.ViewModels
{
    public class OrdersGridViewModel : BindableBase
    {
        private IList<OrderViewModel> orders;
        public IList<OrderViewModel> Orders
        {
            get => orders;
            set => SetProperty(ref orders, value);
        }

        private OrderViewModel selectedOrder;
        public OrderViewModel SelectedOrder
        {
            get => selectedOrder;
            set
            {
                SetProperty(ref selectedOrder, value);

                if (SelectedOrder != null)
                    eventAggregator.ExecuteSafety(() =>
                    {
                        IList<RecipeDTO> recipesOfSelectedOrderDTO = orderingService.GetRecipesOfOrder(SelectedOrder.Id);
                        SelectedOrder.Recipes = new ObservableCollection<RecipeViewModel>();
                        foreach (var r in recipesOfSelectedOrderDTO)
                        {
                            SelectedOrder.Recipes.Add(new RecipeViewModel(r, SelectedOrder.Recipes.Count + 1));
                        }

                        IList<MountDTO> componentsOfSeletedOrderDTOs = orderingService.GetComponentsOfOrder(SelectedOrder.Id);
                        SelectedOrder.Mounts = new ObservableCollection<MountViewModel>();
                        foreach (var c in componentsOfSeletedOrderDTOs.OrderBy(dto => dto.ComponentName))
                        {
                            SelectedOrder.Mounts.Add(new MountViewModel(c, SelectedOrder.Mounts.Count + 1));
                        }

                        var payload = new OrderSelectedPayload { Mounts = SelectedOrder.Mounts };
                        eventAggregator.GetEvent<OrderSelectedEvent>().Publish(payload);
                    });

                EditOrder.RaiseCanExecuteChanged();
                DeleteOrder.RaiseCanExecuteChanged();
                MakeExcel.RaiseCanExecuteChanged();
                Print.RaiseCanExecuteChanged();
            }
        }

        public OrderSearchCriteriaChangedPayload OrderSearchCriteriaPayload { get; private set; }
        public PaginationViewModel OrdersPagination { get; private set; }

        private readonly IEventAggregator eventAggregator;
        private readonly IOrderingService orderingService;
        private readonly IUnityContainer unityContainer;

        private SubscriptionToken OrderSearchCriteriaChangedToken;
        private SubscriptionToken GoToThePageToken;
        private SubscriptionToken OrderAddedEditedToken;

        public OrdersGridViewModel(IEventAggregator eventAggregator, IOrderingService orderingService, IUnityContainer unityContainer)
        {
            this.eventAggregator = eventAggregator;
            this.orderingService = orderingService;
            this.unityContainer = unityContainer;

            this.OrdersPagination = new PaginationViewModel(eventAggregator, PaginationViewModel.DefaultPageSize);
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() => SetOrders());

                OrderSearchCriteriaChangedToken = eventAggregator.GetEvent<OrderSearchCriteriaChangedEvent>().Subscribe(payload =>
                {
                    OrderSearchCriteriaPayload = payload;
                    SetOrders();
                });

                GoToThePageToken = eventAggregator.GetEvent<GoToThePageEvent>().Subscribe(pageNo => SetOrders(pageNo));

                OrderAddedEditedToken = eventAggregator.GetEvent<OrderAddedEditedEvent>().Subscribe(() =>
                {
                    var selectedOrderId = SelectedOrder != null ? SelectedOrder.Id : 0;
                    SetOrders(OrdersPagination.PageNo, selectedOrderId);
                });
            }));
        }

        private DelegateCommand addOrder;
        public DelegateCommand AddOrder
        {
            get => addOrder ?? (addOrder = new DelegateCommand(() =>
            {
                unityContainer.RegisterInstance(UnityNames.AddEditOrderWindowMode, WindowMode.Addition);
                eventAggregator.GetEvent<ShowAddEditOrderWindowEvent>().Publish();
            }));
        }

        private DelegateCommand editOrder;
        public DelegateCommand EditOrder
        {
            get => editOrder ?? (editOrder = new DelegateCommand(() =>
            {
                unityContainer.RegisterInstance(UnityNames.SelectedOrderForEdition, new OrderViewModel(SelectedOrder));
                unityContainer.RegisterInstance(UnityNames.AddEditOrderWindowMode, WindowMode.Edition);
                eventAggregator.GetEvent<ShowAddEditOrderWindowEvent>().Publish();
            },

            () => SelectedOrder != null));
        }

        private DelegateCommand deleteOrder;
        public DelegateCommand DeleteOrder
        {
            get => deleteOrder ?? (deleteOrder = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    if ((bool)DeleteOrderQuestionAction?.Invoke(SelectedOrder.Name))
                    {
                        orderingService.DeleteOrder(SelectedOrder.Id);
                        OrderDeletedAction?.Invoke(SelectedOrder.Name);

                        SetOrders(OrdersPagination.PageNo);
                        eventAggregator.GetEvent<OrderDeletedEvent>().Publish();
                    }
                });
            },

            () => SelectedOrder != null));
        }

        private DelegateCommand makeExcel;
        public DelegateCommand MakeExcel
        {
            get => makeExcel ?? (makeExcel = new DelegateCommand(() =>
            {
                var reportPath = ExportToExcelAction?.Invoke();
                if (!string.IsNullOrWhiteSpace(reportPath))
                {
                    ExcelReports.MakeSelectedOrderReport(reportPath, SelectedOrder);
                    ExcelDataExportedAction?.Invoke(SelectedOrder.Name);
                }
            },

            () => SelectedOrder != null));
        }

        private DelegateCommand print;
        public DelegateCommand Print
        {
            get => print ?? (print = new DelegateCommand(() =>
            {
                //  Printing hidden, for later use maybe.
            },

            () => SelectedOrder != null));
        }

        private void SetOrders(int pageNo = 1, int? selectedOrderId = null)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    OrderSearchCriteriaDTO searchCriteria = null;
                    if (OrderSearchCriteriaPayload != null)
                        searchCriteria = new OrderSearchCriteriaDTO
                        {
                            ComponentName = OrderSearchCriteriaPayload.ComponentName,
                            OrderName = OrderSearchCriteriaPayload.OrderName,
                            OrderDate = OrderSearchCriteriaPayload.OrderDate,
                            OrderNo = OrderSearchCriteriaPayload.OrderNo
                        };

                    OrdersPagination.PageCount = orderingService.GetOrdersCount(pageNo, OrdersPagination.PageSize, searchCriteria);
                    OrdersPagination.PageNo = pageNo;

                    IList<OrderDTO> ordersDTOs = orderingService.GetOrders(pageNo, OrdersPagination.PageSize, searchCriteria);

                    Orders = new ObservableCollection<OrderViewModel>();
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (OrderDTO dto in ordersDTOs)
                            Orders.Add(new OrderViewModel(dto, orders.Count + 1));

                        if (selectedOrderId != null)
                            SelectedOrder = Orders.SingleOrDefault(o => o.Id == selectedOrderId.Value);
                    }));
                });
            });
        }

        public void UnsubscribeEvents()
        {
            eventAggregator.GetEvent<OrderSearchCriteriaChangedEvent>().Unsubscribe(OrderSearchCriteriaChangedToken);
            eventAggregator.GetEvent<GoToThePageEvent>().Unsubscribe(GoToThePageToken);
            eventAggregator.GetEvent<OrderAddedEditedEvent>().Unsubscribe(OrderAddedEditedToken);
        }

        public Func<string, bool> DeleteOrderQuestionAction { get; set; }
        public Action<string> OrderDeletedAction { get; set; }
        public Func<string> ExportToExcelAction { get; set; }
        public Action<string> ExcelDataExportedAction { get; set; }
    }
}
