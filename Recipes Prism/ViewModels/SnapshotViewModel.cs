using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Recipes_Model.DTO;

namespace Recipes_Prism.ViewModels
{
    public class SnapshotViewModel : BindableBase
    {
        public int No { get; set; }
        public string Path { get; set; }
        public DateTime FullDate { get; set; }

        public SnapshotViewModel(SnapshotDTO dto, int no)
        {
            No = no;
            Path = dto.Path;
            FullDate = dto.FullDate;
        }
    }
}
