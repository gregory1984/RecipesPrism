using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Recipes_Model.DTO;

namespace Recipes_Prism.ViewModels
{
    public class RecipeViewModel : BindableBase
    {
        public int? Id { get; set; }
        public int No { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int MeasureId { get; set; }
        public string MeasureName { get; set; }

        private decimal measureCount = decimal.Zero;
        public decimal MeasureCount
        {
            get => measureCount;
            set
            {
                SetProperty(ref measureCount, value);
                RequiredDataCollectedAction?.Invoke();
            }
        }

        public string MeasureCountFormatted
        {
            get => string.Format("{0}", (int)MeasureCount);
        }

        private bool isChecked = false;
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                SetProperty(ref isChecked, value);
                RequiredDataCollectedAction?.Invoke();
            }
        }

        public RecipeViewModel(RecipeDTO dto, int no)
        {
            Id = dto.Id;
            No = no;
            MeasureCount = Math.Round(dto.MeasureCount, 2);
            MeasureId = dto.MeasureId;
            MeasureName = dto.MeasureName;
            ProductId = dto.ProductId;
            ProductName = dto.ProductName;
        }

        public RecipeViewModel(RecipeViewModel vm)
        {
            Id = vm.Id;
            No = vm.No;
            MeasureCount = vm.MeasureCount;
            MeasureId = vm.MeasureId;
            MeasureName = vm.MeasureName;
            ProductId = vm.ProductId;
            ProductName = vm.ProductName;
        }

        /// <summary>
        /// Event fired on IsChecked and ItemCount changes - data required for properly product mount.
        /// </summary>
        public Action RequiredDataCollectedAction { get; set; }
    }
}
