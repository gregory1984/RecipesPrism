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
using Recipes_Prism.Helpers;
using Recipes_Prism.Enums;
using Recipes_Model.Interfaces;
using Recipes_Model.DTO;
using Recipes_Model.DTO.Searching;

namespace Recipes_Prism.ViewModels
{
    public class AboutWindowViewModel : BindableBase
    {
        private string versionNumber = "";
        public string VersionNumber
        {
            get => versionNumber;
            set => SetProperty(ref versionNumber, value);
        }

        private string compilationMarker = "";
        public string CompilationMarker
        {
            get => compilationMarker;
            set => SetProperty(ref compilationMarker, value);
        }

        private string technologies = "";
        public string Technologies
        {
            get => technologies;
            set => SetProperty(ref technologies, value);
        }

        private string author = "";
        public string Author
        {
            get => author;
            set => SetProperty(ref author, value);
        }

        private readonly IUnityContainer unityContainer;

        public AboutWindowViewModel(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                var versionData = unityContainer.Resolve<VersionData>(UnityNames.VersionData);
                VersionNumber = versionData.VersionNumber;
                CompilationMarker = versionData.CompilationMarker;
                Technologies = versionData.Technologies;
                Author = versionData.AuthorsData;
            }));
        }

        private DelegateCommand closeWindow;
        public DelegateCommand CloseWindow
        {
            get => closeWindow ?? (closeWindow = new DelegateCommand(() => CloseWindowAction?.Invoke()));
        }

        public Action CloseWindowAction { get; set; }
    }
}
