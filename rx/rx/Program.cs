using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace rx
{
    class Program
    {
        static void Main(string[] args)
        {
            var subject = new Subject<string>();
            using (subject.Subscribe(new AnonymousBobserver<string>(s => Console.WriteLine(s), () => Console.WriteLine("Done"))))
            {
                var wc = new WebClient();
                var task = wc.DownloadStringTaskAsync("http://www.google.com/robots.txt");
                task.ContinueWith(t =>
                {
                    subject.OnNext(t.Result);
                    subject.OnCompleted();
                });

                // Wait for the async call
                Console.ReadLine();
            }
        }
    }

    internal class AnonymousBobserver<T> : IBobserver<T>
    {
        private readonly Action<T> onNext;
        private readonly Action onCompleted;

        public AnonymousBobserver(Action<T> onNext, Action onCompleted)
        {
            this.onNext = onNext;
            this.onCompleted = onCompleted;
        }

        public void OnNext(T result)
        {
            onNext(result);
        }

        public void OnCompleted()
        {
            onCompleted();
        }
    }

    public class Subject<T>
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
    }

    public interface IBobserver<T>
    {
        void OnNext(T result);
        void OnCompleted();
    }
}
