using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using Recipes_Prism.Events;
using Recipes_Prism.Events.Payloads;
using Recipes_Prism.Helpers;
using Recipes_Model.Interfaces;
using Recipes_Model.DTO;

namespace Recipes_Prism.ViewModels
{
    public class DictionaryEditWindowViewModel : BindableBase
    {
        private string description;
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        private string currentName;
        public string CurrentName
        {
            get => currentName;
            set => SetProperty(ref currentName, value);
        }

        private string newName = "";
        public string NewName
        {
            get => newName;
            set
            {
                SetProperty(ref newName, value);
                Accept.RaiseCanExecuteChanged();
            }
        }

        private DictionaryBaseViewModel selectedDictionary;
        private string selectedDictionaryType;

        private readonly IDictionariesService dictionariesService;
        private readonly IUnityContainer unityContainer;
        private readonly IEventAggregator eventAggregator;

        public DictionaryEditWindowViewModel(IDictionariesService dictionariesService, IUnityContainer unityContainer, IEventAggregator eventAggregator)
        {
            this.dictionariesService = dictionariesService;
            this.unityContainer = unityContainer;
            this.eventAggregator = eventAggregator;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    selectedDictionary = unityContainer.Resolve<DictionaryBaseViewModel>(UnityNames.SelectedDictionaryForEdition);
                    SetDescription();
                    SetNames();
                });
            }));
        }

        private DelegateCommand accept;
        public DelegateCommand Accept
        {
            get => accept ?? (accept = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    if (selectedDictionary is ProductViewModel)
                    {
                        dictionariesService.AddEditProduct(new ProductDTO { Id = selectedDictionary.Id, Name = NewName });
                        eventAggregator.GetEvent<ProductsDictionaryUpdatedEvent>().Publish();
                    }
                    else if (selectedDictionary is ComponentViewModel)
                    {
                        dictionariesService.AddEditComponent(new ComponentDTO { Id = selectedDictionary.Id, Name = NewName });
                        if (selectedDictionary.WasEditedFromMountsForm)
                        {
                            selectedDictionary.Name = NewName;
                            dictionariesService.ForceRefreshComponentsDictionary();

                            var payload = new ComponentOfSelectedMountUpdatedPayload { Component = selectedDictionary as ComponentViewModel };
                            eventAggregator.GetEvent<ComponentOfSelectedMountUpdatedEvent>().Publish(payload);
                        }
                        else
                            eventAggregator.GetEvent<ComponentsDictionaryUpdatedEvent>().Publish();
                    }
                    else if (selectedDictionary is MeasureViewModel)
                    {
                        dictionariesService.AddEditMeasure(new MeasureDTO { Id = selectedDictionary.Id, Name = NewName });
                        eventAggregator.GetEvent<MeasuresDictionaryUpdatedEvent>().Publish();
                    }

                    DictionaryUpdatedAction?.Invoke(selectedDictionaryType);
                });
            },

            () =>
            {
                if (selectedDictionary is ProductViewModel)
                    return !dictionariesService.ProductExists(NewName);
                else if (selectedDictionary is ComponentViewModel)
                    return !dictionariesService.ComponentExists(NewName);
                else if (selectedDictionary is MeasureViewModel)
                    return !dictionariesService.MeasureExists(NewName);
                return false;
            }));
        }

        private void SetDescription()
        {
            if (selectedDictionary is ProductViewModel) selectedDictionaryType = "produktu";
            else if (selectedDictionary is ComponentViewModel) selectedDictionaryType = "składnika";
            else if (selectedDictionary is MeasureViewModel) selectedDictionaryType = "miary";

            Description = "Jeżeli nazwa " + selectedDictionaryType + " zostanie zmieniona na taką, która " +
                          "już istnieje w bazie danych przycisk zmiany nazwy " +
                          "pozostanie nieaktywny.";
        }

        private void SetNames()
        {
            CurrentName = string.Format("Bieżąca nazwa: [ {0} ]", selectedDictionary.Name);
            NewName = selectedDictionary.Name;
        }

        public Action<Exception> ExceptionOccuredAction { get; set; }
        public Action<string> DictionaryUpdatedAction { get; set; }
    }
}
