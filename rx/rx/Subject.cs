using System;
using System.Collections.Generic;

namespace rx
{
    public class Subject<T> : ISubject<T>
    {
        private readonly IList<IBobserver<T>> observers = new List<IBobserver<T>>();

        public IDisposable Subscribe(IBobserver<T> observer)
        {
            observers.Add(observer);

            return new Disposable(() => observers.Remove(observer));
        }

        public void OnNext(T result)
        {
            foreach (var observer in observers)
            {
                observer.OnNext(result);
            }
        }

        public void OnCompleted()
        {
            foreach (var observer in observers)
            {
                observer.OnCompleted();
            }
        }

        public void OnError(Exception exception)
        {
            foreach (var observer in observers)
            {
                observer.OnError(exception);
            }
        }
    }
}