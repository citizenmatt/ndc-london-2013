using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rx
{
    class Program
    {
        private static void Main(string[] args)
        {
            //var wc = new WebClient();
            //var observable = Bobservable.FromTask(() => wc.DownloadStringTaskAsync("http://www.google.com/robots.txt"));
            //var subscription = observable.Subscribe(new AnonymousBobserver<string>(s => Console.WriteLine(s),
            //    () => Console.WriteLine("Done"), exception => Console.WriteLine(exception)));

            var observable = Bobservable.Timer(TimeSpan.FromSeconds(1));
            var subscription = observable.Subscribe(new AnonymousBobserver<long>(l => Console.WriteLine(l), () => Console.WriteLine("Done"),
                e => Console.WriteLine(e)));

            // Wait for the async call
            Console.ReadLine();

            subscription.Dispose();
        }
    }

    public static class Bobservable
    {
        public static IBobservable<long> Timer(TimeSpan interval)
        {
            return new AnonymousBobservable<long>(bobserver =>
            {
                var tick = 0;
                var timer = new Timer(_ => bobserver.OnNext(++tick), null, interval, interval);

                return timer;
            });
        }

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

        public static IBobservable<T> Return<T>(T value)
        {
            return new AnonymousBobservable<T>(bobserver =>
            {
                bobserver.OnNext(value);
                bobserver.OnCompleted();

                return new Disposable(() => {});
            });
        }

        public static IBobservable<T> Throws<T>(Exception exception)
        {
            return new AnonymousBobservable<T>(bobserver =>
            {
                bobserver.OnError(exception);

                return new Disposable(() => { });
            });
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
