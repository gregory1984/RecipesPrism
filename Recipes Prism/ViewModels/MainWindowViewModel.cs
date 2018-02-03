using System;
using Prism.Mvvm;
using Prism.Commands;
using Prism.Events;
using Recipes_Prism.Events;
using Recipes_Prism.Helpers;
using Recipes_Model.Interfaces;

namespace Recipes_Prism.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IDatabaseService databaseService;

        public MainWindowViewModel(IEventAggregator eventAggregator, IDatabaseService databaseService)
        {
            this.eventAggregator = eventAggregator;
            this.databaseService = databaseService;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    databaseService.CheckConnection();
                    databaseService.Initialize();
                });

                eventAggregator.GetEvent<ExceptionOccuredEvent>().Subscribe(ex => ExceptionOccuredAction?.Invoke(ex));
                eventAggregator.GetEvent<ShowDictionariesWindowEvent>().Subscribe(() => ShowDictionariesWindowAction?.Invoke());
                eventAggregator.GetEvent<ShowMountingWindowEvent>().Subscribe(() => ShowMountingWindowAction?.Invoke());
                eventAggregator.GetEvent<QuitApplicationEvent>().Subscribe(() => QuitApplicationAction?.Invoke());
                eventAggregator.GetEvent<ShowAddEditOrderWindowEvent>().Subscribe(() => ShowAddEditOrderWindowAction?.Invoke());
                eventAggregator.GetEvent<ShowBackupWindowEvent>().Subscribe(() => ShowBackupWindowAction?.Invoke());
                eventAggregator.GetEvent<ShowAboutWindowEvent>().Subscribe(() => ShowAboutWindowAction?.Invoke());
                eventAggregator.GetEvent<RestartApplicationEvent>().Subscribe(() => RestartApplicationAction?.Invoke());
            }));
        }

        public Action<Exception> ExceptionOccuredAction { get; set; }
        public Action ShowDictionariesWindowAction { get; set; }
        public Action ShowMountingWindowAction { get; set; }
        public Action ShowAddEditOrderWindowAction { get; set; }
        public Action QuitApplicationAction { get; set; }
        public Action ShowBackupWindowAction { get; set; }
        public Action ShowAboutWindowAction { get; set; }
        public Action RestartApplicationAction { get; set; }
    }
}
