using Prism.Events;
using Recipes_Prism.Events.Payloads;

namespace Recipes_Prism.Events
{
    public class PrimaryOrderDataCollectedEvent : PubSubEvent<PrimaryOrderDataCollectedPayload> { }
}
