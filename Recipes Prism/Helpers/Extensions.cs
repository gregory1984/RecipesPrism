using System;
using System.Collections.Generic;
using Prism.Events;
using Recipes_Prism.Events;

namespace Recipes_Prism.Helpers
{
    public static class Extensions
    {
        public static void ExecuteSafety(this IEventAggregator eventAggregator, Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                eventAggregator.GetEvent<ExceptionOccuredEvent>().Publish(ex);
            }
        }

        public static T ExecuteSafetyWithResult<T>(this IEventAggregator eventAggregator, Func<T> function)
        {
            try
            {
                return function();
            }
            catch (Exception ex)
            {
                eventAggregator.GetEvent<ExceptionOccuredEvent>().Publish(ex);
            }
            return default(T);
        }
    }
}
