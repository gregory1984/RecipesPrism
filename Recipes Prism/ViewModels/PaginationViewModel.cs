using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Recipes_Prism.Events.Pagination;

namespace Recipes_Prism.ViewModels
{
    public class PaginationViewModel : BindableBase
    {
        private int pageNo = 1;
        public int PageNo
        {
            get => pageNo;
            set
            {
                SetProperty(ref pageNo, value);
                GoFirstPage.RaiseCanExecuteChanged();
                GoPreviousPage.RaiseCanExecuteChanged();
                GoNextPage.RaiseCanExecuteChanged();
                GoLastPage.RaiseCanExecuteChanged();
            }
        }

        private int pageCount = 0;
        public int PageCount
        {
            get => pageCount;
            set => SetProperty(ref pageCount, value);
        }

        public static int DefaultPageSize { get; } = 30;
        public int PageSize { get; private set; } = DefaultPageSize;

        private DelegateCommand goFirstPage;
        public DelegateCommand GoFirstPage
        {
            get => goFirstPage ?? (goFirstPage = new DelegateCommand(() =>
            {
                PageNo = 1;
                eventAggregator.GetEvent<GoToThePageEvent>().Publish(pageNo);
            },

            () => PageNo > 1));
        }

        private DelegateCommand goPreviousPage;
        public DelegateCommand GoPreviousPage
        {
            get => goPreviousPage ?? (goPreviousPage = new DelegateCommand(() =>
            {
                PageNo -= 1;
                eventAggregator.GetEvent<GoToThePageEvent>().Publish(pageNo);
            },

            () => PageNo > 1));
        }

        private DelegateCommand goNextPage;
        public DelegateCommand GoNextPage
        {
            get => goNextPage ?? (goNextPage = new DelegateCommand(() =>
            {
                PageNo += 1;
                eventAggregator.GetEvent<GoToThePageEvent>().Publish(pageNo);
            },

            () => PageNo < PageCount));
        }

        private DelegateCommand goLastPage;
        public DelegateCommand GoLastPage
        {
            get => goLastPage ?? (goLastPage = new DelegateCommand(() =>
            {
                PageNo = PageCount;
                eventAggregator.GetEvent<GoToThePageEvent>().Publish(pageNo);
            },

            () => PageNo < PageCount));
        }

        private readonly IEventAggregator eventAggregator;

        public PaginationViewModel(IEventAggregator eventAggregator, int pageSize)
        {
            this.eventAggregator = eventAggregator;
            this.PageSize = pageSize;
        }
    }
}
