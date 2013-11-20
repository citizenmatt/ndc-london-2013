using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace rx
{
    class Program
    {
        private static void Main(string[] args)
        {
            var wc = new WebClient();
            var observable = Bobservable.FromTask(() => wc.DownloadStringTaskAsync("http://www.google.com/robots.txt"));
            var subscription = observable.Subscribe(new AnonymousBobserver<string>(s => Console.WriteLine(s),
                () => Console.WriteLine("Done"), exception => Console.WriteLine(exception)));

            // Wait for the async call
            Console.ReadLine();

            subscription.Dispose();
        }
    }

    public class Bobservable
    {
        public static IBobservable<T> FromTask<T>(Func<Task<T>> taskCreator)
        {
            return new AnonymousBobservable<T>(bobserver =>
            {
                var unsubscribed = false;

                taskCreator().ContinueWith(t =>
                {
                    if (unsubscribed)
                        return;

                    if (t.IsFaulted)
                        bobserver.OnError(t.Exception);
                    else
                    {
                        bobserver.OnNext(t.Result);
                        bobserver.OnCompleted();
                    }
                });

                return new Disposable(() => { unsubscribed = true; });
            });
        }
    }

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

    public interface IBobservable<T>
    {
        IDisposable Subscribe(IBobserver<T> observer);
    }

    public interface IBobserver<T>
    {
        void OnNext(T result);
        void OnCompleted();
        void OnError(Exception exception);
    }
}
