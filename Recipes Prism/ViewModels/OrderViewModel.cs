using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Recipes_Model.DTO;

namespace Recipes_Prism.ViewModels
{
    public class OrderViewModel : BindableBase
    {
        public int Id { get; set; }
        public int No { get; set; }

        private string name = "";
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string orderNo = "";
        public string OrderNo
        {
            get => orderNo;
            set => SetProperty(ref orderNo, value);
        }

        private decimal itemCount = decimal.Zero;
        public decimal ItemCount
        {
            get => itemCount;
            set => SetProperty(ref itemCount, value);
        }

        public string ItemCountFormatted
        {
            get => string.Format("{0}", (int)ItemCount);
        }

        private string comments = "";
        public string Comments
        {
            get => comments;
            set => SetProperty(ref comments, value);
        }

        private DateTime date;
        public DateTime Date
        {
            get => date;
            set => SetProperty(ref date, value);
        }

        private IList<RecipeViewModel> recipes;
        public IList<RecipeViewModel> Recipes
        {
            get => recipes;
            set => SetProperty(ref recipes, value);
        }

        private IList<MountViewModel> mounts;
        public IList<MountViewModel> Mounts
        {
            get => mounts;
            set => SetProperty(ref mounts, value);
        }

        public OrderViewModel(OrderDTO dto, int no)
        {
            Id = dto.Id.Value;
            No = no;
            Date = dto.Date;
            ItemCount = Math.Round(dto.ItemCount, 2);
            Name = dto.Name;
            OrderNo = dto.OrderNo;
            Comments = dto.Comments;
        }

        public OrderViewModel(OrderViewModel vm)
        {
            Id = vm.Id;
            No = vm.No;
            Date = vm.Date;
            ItemCount = vm.ItemCount;
            Name = vm.Name;
            OrderNo = vm.OrderNo;
            Comments = vm.Comments;

            Recipes = new ObservableCollection<RecipeViewModel>();
            foreach (var r in vm.Recipes)
                Recipes.Add(new RecipeViewModel(r));

            Mounts = new ObservableCollection<MountViewModel>();
            foreach (var m in vm.Mounts)
                Mounts.Add(new MountViewModel(m));
        }
    }
}
