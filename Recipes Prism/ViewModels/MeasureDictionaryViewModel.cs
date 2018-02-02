using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Data;
using System.Threading;
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
    public class MeasureDictionaryViewModel : BindableBase
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

        private ICollectionView measuresView;
        public ICollectionView MeasuresView
        {
            get => measuresView;
            set => SetProperty(ref measuresView, value);
        }

        private MeasureViewModel selectedMeasure;
        public MeasureViewModel SelectedMeasure
        {
            get => selectedMeasure;
            set => SetProperty(ref selectedMeasure, value);
        }

        private readonly IEventAggregator eventAggregator;
        private readonly IDictionariesService dictionariesService;
        private readonly IUnityContainer unityContainer;
        private SubscriptionToken MeasuresDictionaryUpdatedToken;

        public MeasureDictionaryViewModel(IEventAggregator eventAggregator, IDictionariesService dictionariesService, IUnityContainer unityContainer)
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
                SetMeasures();
                MeasuresDictionaryUpdatedToken = eventAggregator.GetEvent<MeasuresDictionaryUpdatedEvent>().Subscribe(() => SetMeasures(dictionaryUpdate: true));
            }));
        }

        private DelegateCommand find;
        public DelegateCommand Find { get => find ?? (find = new DelegateCommand(() => MeasuresView.Refresh())); }

        private DelegateCommand add;
        public DelegateCommand Add
        {
            get => add ?? (add = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    dictionariesService.AddEditMeasure(new MeasureDTO { Name = Name });
                    SetMeasures(dictionaryUpdate: true);
                    Name = "";
                });
            },

            () => eventAggregator.ExecuteSafetyWithResult<bool>(() => !dictionariesService.ProductExists(Name)) && !string.IsNullOrWhiteSpace(Name)));
        }

        private DelegateCommand edit;
        public DelegateCommand Edit
        {
            get => edit ?? (edit = new DelegateCommand(() =>
            {
                unityContainer.RegisterInstance(UnityNames.SelectedDictionaryForEdition, SelectedMeasure as DictionaryBaseViewModel);
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
                    dictionariesService.DeleteMeasure(SelectedMeasure.Id);
                    SetMeasures(dictionaryUpdate: true);
                });
            }));
        }

        internal void SetMeasures(bool dictionaryUpdate = false)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    IList<MeasureWithCountsDTO> measuresDTOs = dictionariesService.GetMeasuresWithMountsCheck(dictionaryUpdate);
                    IList<MeasureViewModel> measures = new List<MeasureViewModel>();
                    foreach (var dto in measuresDTOs)
                        measures.Add(new MeasureViewModel(dto, measures.Count + 1));

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MeasuresView = CollectionViewSource.GetDefaultView(measures);
                        MeasuresView.Filter = (object item) => (item as MeasureViewModel).Name.ToLower().Contains(Name.ToLower());
                    }));
                });
            });
        }

        public void UnsubscribeEvents()
        {
            eventAggregator.GetEvent<MeasuresDictionaryUpdatedEvent>().Unsubscribe(MeasuresDictionaryUpdatedToken);
        }
    }
}
