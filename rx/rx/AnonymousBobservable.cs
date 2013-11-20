using System;

namespace rx
{
    public class AnonymousBobservable<T> : IBobservable<T>
    {
        private readonly Func<IBobserver<T>, IDisposable> onSubscribe;

        public AnonymousBobservable(Func<IBobserver<T>, IDisposable> onSubscribe)
        {
            this.onSubscribe = onSubscribe;
        }

        public IDisposable Subscribe(IBobserver<T> observer)
        {
            return onSubscribe(observer);
        }
    }
}