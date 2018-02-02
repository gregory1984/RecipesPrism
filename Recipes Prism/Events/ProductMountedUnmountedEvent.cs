using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Recipes_Prism.ViewModels;
using Recipes_Prism.Events.Payloads;

namespace Recipes_Prism.Events
{
    public class ProductMountedUnmountedEvent : PubSubEvent<ProductMountedUnmountedPayload> { }
}
