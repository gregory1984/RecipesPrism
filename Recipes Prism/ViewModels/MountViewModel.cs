using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Recipes_Model.DTO;
using Recipes_Model.Interfaces;

namespace Recipes_Prism.ViewModels
{
    public class MountViewModel : BindableBase
    {
        public int No { get; set; }
        public int ComponentId { get; set; }
        public string MeasureName { get; set; }

        private string componentName;
        public string ComponentName
        {
            get => componentName;
            set => SetProperty(ref componentName, value);
        }

        private bool isChecked;
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                SetProperty(ref isChecked, value);
                RequiredDataCollectedAction?.Invoke();
            }
        }

        private bool canBeDeleted;
        public bool CanBeDeleted
        {
            get => canBeDeleted;
            set => SetProperty(ref canBeDeleted, value);
        }

        private IList<MeasureViewModel> measures;
        public IList<MeasureViewModel> Measures
        {
            get => measures;
            set => SetProperty(ref measures, value);
        }

        private MeasureViewModel selectedMeasure;
        public MeasureViewModel SelectedMeasure
        {
            get => selectedMeasure;
            set => SetProperty(ref selectedMeasure, value);
        }

        private decimal measureCount;
        public decimal MeasureCount
        {
            get => measureCount;
            set => SetProperty(ref measureCount, value);
        }

        private decimal itemCount;
        public decimal ItemCount
        {
            get => itemCount;
            set
            {
                SetProperty(ref itemCount, value);
                RequiredDataCollectedAction?.Invoke();
            }
        }

        public MountViewModel(IDictionariesService dictionaryService, MountDTO dto, int no)
        {
            No = no;
            ComponentId = dto.ComponentId;
            ComponentName = dto.ComponentName;
            MeasureName = dto.MeasureName;
            IsChecked = dto.IsComponentMounted;
            CanBeDeleted = !dto.IsComponentMounted;
            MeasureCount = Math.Round(dto.MeasureCount, 2);
            ItemCount = Math.Round(dto.ItemCount, 2);

            Measures = new ObservableCollection<MeasureViewModel>();
            foreach (var m in dictionaryService.GetMeasuresWithMountsCheck())
            {
                Measures.Add(new MeasureViewModel(m));
            }
            SelectedMeasure = Measures.SingleOrDefault(m => m.Id == dto.MeasureId);
        }

        public MountViewModel(MountViewModel vm)
        {
            No = vm.No;
            ComponentId = vm.ComponentId;
            ComponentName = vm.ComponentName;
            MeasureName = vm.MeasureName;
            IsChecked = vm.IsChecked;
            CanBeDeleted = vm.CanBeDeleted;
            MeasureCount = vm.MeasureCount;
            ItemCount = vm.ItemCount;

            //Measures = new ObservableCollection<MeasureViewModel>();
            //foreach (var m in vm.Measures)
            //    Measures.Add(new MeasureViewModel(m));

            //SelectedMeasure = Measures.SingleOrDefault(m => m.Id == vm.SelectedMeasure.Id);
        }

        public MountViewModel(MountDTO dto, int no)
        {
            No = no;
            ComponentName = dto.ComponentName;
            MeasureCount = Math.Round(dto.MeasureCount, 2);
            MeasureName = dto.MeasureName;
            ItemCount = Math.Round(dto.ItemCount, 2);
        }

        /// <summary>
        /// Event fired on IsChecked and ItemCount changes - data required for properly product mount.
        /// </summary>
        public Action RequiredDataCollectedAction { get; set; }
    }
}
