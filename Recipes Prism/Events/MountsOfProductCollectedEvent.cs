using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Recipes_Prism.Events.Payloads;

namespace Recipes_Prism.Events
{
    /// <summary>
    /// Event fired on mount set (IsChecked and ItemCount != decimal.Zero) inside mounting form.
    /// </summary>
    public class MountsOfProductCollectedEvent : PubSubEvent<MountsOfProductCollectedPayload> { }
}
