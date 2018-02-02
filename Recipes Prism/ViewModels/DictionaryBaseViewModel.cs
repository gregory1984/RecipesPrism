using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recipes_Prism.ViewModels
{
    public class DictionaryBaseViewModel : BindableBase
    {
        public int Id { get; set; }
        public bool CanBeDeleted { get; set; }
        public bool WasEditedFromMountsForm { get; set; }

        private int no;
        public int No
        {
            get => no;
            set => SetProperty(ref no, value);
        }

        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
    }
}
