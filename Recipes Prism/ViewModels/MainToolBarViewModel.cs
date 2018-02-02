using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Recipes_Prism.Events;

namespace Recipes_Prism.ViewModels
{
    public class MainToolBarViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;

        public MainToolBarViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        private DelegateCommand showDictionariesWindow;
        public DelegateCommand ShowDictionariesWindow
        {
            get => showDictionariesWindow ?? (showDictionariesWindow = new DelegateCommand(() => eventAggregator.GetEvent<ShowDictionariesWindowEvent>().Publish()));
        }

        private DelegateCommand showMountingWindow;
        public DelegateCommand ShowMountingWindow
        {
            get => showMountingWindow ?? (showMountingWindow = new DelegateCommand(() => eventAggregator.GetEvent<ShowMountingWindowEvent>().Publish()));
        }

        private DelegateCommand showHideFilters;
        public DelegateCommand ShowHideFilters
        {
            get => showHideFilters ?? (showHideFilters = new DelegateCommand(() => eventAggregator.GetEvent<ShowHideFiltersEvent>().Publish()));
        }

        private DelegateCommand quit;
        public DelegateCommand Quit
        {
            get => quit ?? (quit = new DelegateCommand(() => eventAggregator.GetEvent<QuitApplicationEvent>().Publish()));
        }

        private DelegateCommand showBackupWindow;
        public DelegateCommand ShowBackupWindow
        {
            get => showBackupWindow ?? (showBackupWindow = new DelegateCommand(() => eventAggregator.GetEvent<ShowBackupWindowEvent>().Publish()));
        }

        private DelegateCommand showAboutWindow;
        public DelegateCommand ShowAboutWindow
        {
            get => showAboutWindow ?? (showAboutWindow = new DelegateCommand(() => eventAggregator.GetEvent<ShowAboutWindowEvent>().Publish()));
        }
    }
}
