using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Recipes_Prism.Events;

namespace Recipes_Prism.ViewModels
{
    public class DictionariesWindowViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private SubscriptionToken ShowDictionaryEditWindowToken;

        public DictionariesWindowViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                ShowDictionaryEditWindowToken = eventAggregator.GetEvent<ShowDictionaryEditWindowEvent>().Subscribe(() => ShowDictionaryEditWindowAction?.Invoke());
            }));
        }

        private DelegateCommand closeWindow;
        public DelegateCommand CloseWindow { get => closeWindow ?? (closeWindow = new DelegateCommand(() => CloseWindowAction?.Invoke())); }

        public void UnsubscribePrismEvents()
        {
            eventAggregator.GetEvent<ShowDictionaryEditWindowEvent>().Unsubscribe(ShowDictionaryEditWindowToken);
        }

        public Action<Exception> ExceptionOccuredAction { get; set; }
        public Action ShowDictionaryEditWindowAction { get; set; }
        public Action CloseWindowAction { get; set; }
    }
}
