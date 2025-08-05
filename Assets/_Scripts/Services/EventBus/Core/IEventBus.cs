using System;

namespace _Scripts.Services.EventBus.Core
{
    public interface IEventBus
    {
        IObservable<T> OnEvent<T>();
        void Publish<T>(T evt);
    }
}