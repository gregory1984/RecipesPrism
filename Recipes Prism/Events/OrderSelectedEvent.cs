using Prism.Events;
using Recipes_Prism.ViewModels;
using Recipes_Prism.Events.Payloads;

namespace Recipes_Prism.Events
{
    public class OrderSelectedEvent : PubSubEvent<OrderSelectedPayload> { }
}
