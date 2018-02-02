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
using Recipes_Prism.Events.Pagination;
using Recipes_Prism.Helpers;
using Recipes_Model.Interfaces;
using Recipes_Model.DTO;

namespace Recipes_Prism.ViewModels
{
    public class ComponentsOfProductViewModel : BindableBase
    {
        private string sumGroupedByMeasure = "";
        public string SumGroupedByMeasure
        {
            get => sumGroupedByMeasure;
            set => SetProperty(ref sumGroupedByMeasure, value);
        }

        private string sumUnitIndependent = "";
        public string SumUnitIndependent
        {
            get => sumUnitIndependent;
            set => SetProperty(ref sumUnitIndependent, value);
        }

        private IList<MountViewModel> mounts;
        public IList<MountViewModel> Mounts
        {
            get => mounts;
            set => SetProperty(ref mounts, value);
        }

        private readonly IEventAggregator eventAggregator;
        private readonly IOrderingService orderingService;
        private SubscriptionToken OrderSelectedToken;
        private SubscriptionToken OrderSearchCriteriaChangedToken;
        private SubscriptionToken GoToThePageToken;
        private SubscriptionToken OrderDeletedToken;

        public ComponentsOfProductViewModel(IEventAggregator eventAggregator, IOrderingService orderingService)
        {
            this.eventAggregator = eventAggregator;
            this.orderingService = orderingService;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    OrderSelectedToken = eventAggregator.GetEvent<OrderSelectedEvent>().Subscribe(payload =>
                    {
                        Mounts = new ObservableCollection<MountViewModel>(payload.Mounts);

                        var grouped = Mounts
                            .GroupBy(m => m.MeasureName)
                            .Select(m => new
                            {
                                MeasureName = m.Key,
                                MeasureCount = m.Sum(mount => mount.MeasureCount)
                            }).ToList();

                        SumGroupedByMeasure = string.Join(" ", grouped.Select(g => "[ " + Math.Round(g.MeasureCount, 2).ToString() + g.MeasureName + " ]"));
                        SumUnitIndependent = Math.Round(Mounts.Sum(m => m.MeasureCount), 2).ToString();
                    });

                    OrderSearchCriteriaChangedToken = eventAggregator.GetEvent<OrderSearchCriteriaChangedEvent>().Subscribe(criteria =>
                    {
                        ClearMountsInfo();
                    });

                    //  Clear mounts (components of selected product) on each current page change.
                    GoToThePageToken = eventAggregator.GetEvent<GoToThePageEvent>().Subscribe(pageNo =>
                    {
                        ClearMountsInfo();
                    });

                    OrderDeletedToken = eventAggregator.GetEvent<OrderDeletedEvent>().Subscribe(() =>
                    {
                        ClearMountsInfo();
                    });
                });
            }));
        }

        internal void ClearMountsInfo()
        {
            Mounts = null;
            SumGroupedByMeasure = SumUnitIndependent = "";
        }

        public void UnsubscribeEvents()
        {
            eventAggregator.GetEvent<OrderSelectedEvent>().Unsubscribe(OrderSelectedToken);
            eventAggregator.GetEvent<OrderSearchCriteriaChangedEvent>().Unsubscribe(OrderSearchCriteriaChangedToken);
            eventAggregator.GetEvent<GoToThePageEvent>().Unsubscribe(GoToThePageToken);
            eventAggregator.GetEvent<OrderDeletedEvent>().Unsubscribe(OrderDeletedToken);
        }
    }
}
