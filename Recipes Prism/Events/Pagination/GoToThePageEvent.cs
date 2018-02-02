using Prism.Events;

namespace Recipes_Prism.Events.Pagination
{
    /// <summary>
    /// Go to the page number: <int>
    /// If You need to set paging to multiple grids You have to create one event grid with different names.
    /// </summary>
    public class GoToThePageEvent : PubSubEvent<int> { }
}
