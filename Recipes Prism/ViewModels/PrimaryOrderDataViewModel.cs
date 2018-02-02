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
    public class PrimaryOrderDataViewModel : BindableBase
    {
        private string orderName = "2017";
        public string OrderName
        {
            get => orderName;
            set
            {
                SetProperty(ref orderName, value);
                PublishPrimaryOrderData();
            }
        }

        private string orderNo = "S235JR";
        public string OrderNo
        {
            get => orderNo;
            set
            {
                SetProperty(ref orderNo, value);
                PublishPrimaryOrderData();
            }
        }

        private decimal itemCount = 1;
        public decimal ItemCount
        {
            get => itemCount;
            set
            {
                SetProperty(ref itemCount, value);
                PublishPrimaryOrderData();
            }
        }

        private DateTime? orderDate = DateTime.Today;
        public DateTime? OrderDate
        {
            get => orderDate;
            set
            {
                SetProperty(ref orderDate, value);
                PublishPrimaryOrderData();
            }
        }

        private string comments = "";
        public string Comments
        {
            get => comments;
            set
            {
                SetProperty(ref comments, value);
                PublishPrimaryOrderData();
            }
        }

        private readonly IEventAggregator eventAggregator;
        private readonly IUnityContainer unityContainer;

        private WindowMode windowMode;
        private OrderViewModel selectedOrder;

        public PrimaryOrderDataViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer)
        {
            this.eventAggregator = eventAggregator;
            this.unityContainer = unityContainer;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                windowMode = unityContainer.Resolve<WindowMode>(UnityNames.AddEditOrderWindowMode);
                if (windowMode == WindowMode.Edition)
                {
                    selectedOrder = unityContainer.Resolve<OrderViewModel>(UnityNames.SelectedOrderForEdition);
                    OrderName = selectedOrder.Name;
                    OrderNo = selectedOrder.OrderNo;
                    ItemCount = selectedOrder.ItemCount;
                    OrderDate = selectedOrder.Date;
                    Comments = selectedOrder.Comments;
                }
            }));
        }

        private void PublishPrimaryOrderData()
        {
            var payload = new PrimaryOrderDataCollectedPayload(OrderName, OrderNo, ItemCount, OrderDate, Comments);
            eventAggregator.GetEvent<PrimaryOrderDataCollectedEvent>().Publish(payload);
        }
    }
}
