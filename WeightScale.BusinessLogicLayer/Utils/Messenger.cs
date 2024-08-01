using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WeightScale.BusinessLogicLayer.Models;

namespace WeightScale.BusinessLogicLayer.Utils
{
    public interface IMessenger
    {
        void Send<TMessage>(TMessage message);
        void Subscribe<TMessage>(object subscriber, Action<object> action);
        void Unsubscribe<TMessage>(object subscriber);
    }

    public class Messenger : IMessenger
    {
        private readonly ConcurrentDictionary<Type, object> _currentState = new ConcurrentDictionary<Type, object>();

        private readonly ConcurrentDictionary<Type, List<Subscription>> _subscriptions =
                new ConcurrentDictionary<Type, List<Subscription>>();

        public void Send<TMessage>(TMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!_subscriptions.ContainsKey(typeof(TMessage)))
            {
                _subscriptions.TryAdd(typeof(TMessage), new List<Subscription>());
            }

            _currentState.AddOrUpdate(typeof(TMessage), o => message, (o, old) => message);
            foreach (var subscription in _subscriptions[typeof(TMessage)])
            {
                subscription.Action(message);
            }
        }

        public void Subscribe<TMessage>(object subscriber, Action<object> action)
        {
            if (!_subscriptions.ContainsKey(typeof(TMessage)))
            {
                _subscriptions.TryAdd(typeof(TMessage), new List<Subscription>());
            }

            var newSubscriber = new Subscription(subscriber, action);
            _subscriptions[typeof(TMessage)]
                    .Add(newSubscriber);
        }

        public void Unsubscribe<TMessage>(object subscriber)
        {
            if (!_subscriptions.ContainsKey(typeof(TMessage)))
            {
                return;
            }

            var subscription = _subscriptions[typeof(TMessage)]
                    .FirstOrDefault(x => x.Subscriber.Equals(subscriber));
            if (subscription != null)
            {
                _subscriptions[typeof(TMessage)]
                        .Remove(subscription);
            }
        }
    }
}