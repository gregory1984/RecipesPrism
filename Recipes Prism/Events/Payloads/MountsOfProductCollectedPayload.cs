using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recipes_Prism.ViewModels;

namespace Recipes_Prism.Events.Payloads
{
    public class MountsOfProductCollectedPayload
    {
        public ProductViewModel SelectedProduct { get; set; }
        public IList<MountViewModel> Mounts { get; set; }

        /// <summary>
        /// Determines if product can be properly mount.
        /// </summary>
        public bool IsMountable { get; set; }
    }
}
