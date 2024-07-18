using System;

namespace WeightScale.BusinessLogicLayer.Models
{
    public class Subscription
    {
        public object Subscriber { get; }
        public Action<object> Action { get; }

        public Subscription(object subscriber, Action<object> action)
        {
            Subscriber = subscriber;
            Action = action;
        }
    }
}
