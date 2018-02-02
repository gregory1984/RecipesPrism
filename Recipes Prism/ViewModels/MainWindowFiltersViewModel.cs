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
using Recipes_Prism.Helpers;
using Recipes_Model.Interfaces;
using Recipes_Model.DTO;

namespace Recipes_Prism.ViewModels
{
    public class MainWindowFiltersViewModel : BindableBase
    {
        private string orderName = "";
        public string OrderName
        {
            get => orderName;
            set => SetProperty(ref orderName, value);
        }

        private string orderNo = "";
        public string OrderNo
        {
            get => orderNo;
            set => SetProperty(ref orderNo, value);
        }

        private string componentName = "";
        public string ComponentName
        {
            get => componentName;
            set => SetProperty(ref componentName, value);
        }

        private DateTime? orderDate;
        public DateTime? OrderDate
        {
            get => orderDate;
            set => SetProperty(ref orderDate, value);
        }

        private Visibility filtersVisibility = Visibility.Collapsed;
        public Visibility FiltersVisibility
        {
            get => filtersVisibility;
            set => SetProperty(ref filtersVisibility, value);
        }

        private readonly IEventAggregator eventAggregator;
        private SubscriptionToken ShowHideFiltersToken;

        public MainWindowFiltersViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                ShowHideFiltersToken = eventAggregator.GetEvent<ShowHideFiltersEvent>().Subscribe(() =>
                {
                    FiltersVisibility = FiltersVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
                });
            }));
        }

        private DelegateCommand clearFilters;
        public DelegateCommand ClearFilters
        {
            get => clearFilters ?? (clearFilters = new DelegateCommand(() =>
            {
                ComponentName = OrderName = OrderNo = "";
                OrderDate = null;
                eventAggregator.GetEvent<OrderSearchCriteriaChangedEvent>().Publish(new OrderSearchCriteriaChangedPayload());
            }));
        }

        private DelegateCommand findOrders;
        public DelegateCommand FindOrders
        {
            get => findOrders ?? (findOrders = new DelegateCommand(() =>
            {
                var payload = new OrderSearchCriteriaChangedPayload
                {
                    ComponentName = ComponentName,
                    OrderDate = OrderDate,
                    OrderName = OrderName,
                    OrderNo = OrderNo
                };

                eventAggregator.GetEvent<OrderSearchCriteriaChangedEvent>().Publish(payload);
            }));
        }

        public void UnsubscribeEvents()
        {
            eventAggregator.GetEvent<ShowHideFiltersEvent>().Unsubscribe(ShowHideFiltersToken);
        }
    }
}
