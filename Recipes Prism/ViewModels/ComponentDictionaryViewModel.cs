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
using Recipes_Prism.Helpers;
using Recipes_Model.Interfaces;
using Recipes_Model.DTO;


namespace Recipes_Prism.ViewModels
{
    public class ComponentDictionaryViewModel : BindableBase
    {
        private string name = "";
        public string Name
        {
            get => name;
            set
            {
                SetProperty(ref name, value);
                Add.RaiseCanExecuteChanged();
            }
        }

        private ICollectionView componentsView;
        public ICollectionView ComponentsView
        {
            get => componentsView;
            set => SetProperty(ref componentsView, value);
        }

        private ComponentViewModel selectedComponent;
        public ComponentViewModel SelectedComponent
        {
            get => selectedComponent;
            set => SetProperty(ref selectedComponent, value);
        }

        private readonly IEventAggregator eventAggregator;
        private readonly IDictionariesService dictionariesService;
        private readonly IUnityContainer unityContainer;
        private SubscriptionToken ComponentsDictionaryUpdatedToken;

        public ComponentDictionaryViewModel(IEventAggregator eventAggregator, IDictionariesService dictionariesService, IUnityContainer unityContainer)
        {
            this.eventAggregator = eventAggregator;
            this.dictionariesService = dictionariesService;
            this.unityContainer = unityContainer;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                SetComponents();
                ComponentsDictionaryUpdatedToken = eventAggregator.GetEvent<ComponentsDictionaryUpdatedEvent>().Subscribe(() =>
                {
                    SetComponents(dictionaryUpdate: true);
                });
            }));
        }

        private DelegateCommand find;
        public DelegateCommand Find { get => find ?? (find = new DelegateCommand(() => ComponentsView.Refresh())); }

        private DelegateCommand add;
        public DelegateCommand Add
        {
            get => add ?? (add = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    dictionariesService.AddEditComponent(new ComponentDTO { Name = Name });
                    SetComponents(dictionaryUpdate: true);
                    Name = "";
                });
            },

            () => eventAggregator.ExecuteSafetyWithResult<bool>(() => !dictionariesService.ComponentExists(Name)) && !string.IsNullOrWhiteSpace(Name)));
        }

        private DelegateCommand edit;
        public DelegateCommand Edit
        {
            get => edit ?? (edit = new DelegateCommand(() =>
            {
                unityContainer.RegisterInstance(UnityNames.SelectedDictionaryForEdition, SelectedComponent as DictionaryBaseViewModel);
                eventAggregator.GetEvent<ShowDictionaryEditWindowEvent>().Publish();
            }));
        }

        private DelegateCommand delete;
        public DelegateCommand Delete
        {
            get => delete ?? (delete = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    dictionariesService.DeleteComponent(SelectedComponent.Id);
                    SetComponents(dictionaryUpdate: true);
                });
            }));
        }

        internal void SetComponents(bool dictionaryUpdate = false)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    IList<ComponentWithCountsDTO> componentsDTOs = dictionariesService.GetComponentsWithMountsCheck(dictionaryUpdate);
                    IList<ComponentViewModel> components = new List<ComponentViewModel>();
                    foreach (var dto in componentsDTOs)
                        components.Add(new ComponentViewModel(dto, components.Count + 1));

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ComponentsView = CollectionViewSource.GetDefaultView(components);
                        ComponentsView.Filter = (object item) => (item as ComponentViewModel).Name.ToLower().Contains(Name.ToLower());
                    }));
                });
            });
        }

        public void UnsubscribeEvents()
        {
            eventAggregator.GetEvent<ComponentsDictionaryUpdatedEvent>().Unsubscribe(ComponentsDictionaryUpdatedToken);
        }
    }
}
