using System.Collections.Generic;
using Recipes_Prism.ViewModels;

namespace Recipes_Prism.Events.Payloads
{
    public class OrderSelectedPayload
    {
        public IList<MountViewModel> Mounts { get; set; }
    }
}
