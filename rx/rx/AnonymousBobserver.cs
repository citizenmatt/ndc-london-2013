using System;

namespace rx
{
    internal class AnonymousBobserver<T> : IBobserver<T>
    {
        private readonly Action<T> onNext;
        private readonly Action onCompleted;
        private readonly Action<Exception> onError;

        public AnonymousBobserver(Action<T> onNext, Action onCompleted, Action<Exception> onError)
        {
            this.onNext = onNext;
            this.onCompleted = onCompleted;
            this.onError = onError;
        }

        public void OnNext(T result)
        {
            onNext(result);
        }

        public void OnCompleted()
        {
            onCompleted();
        }

        public void OnError(Exception exception)
        {
            onError(exception);
        }
    }
}