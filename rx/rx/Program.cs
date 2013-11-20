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
            using (subject.Subscribe(result => Console.WriteLine(result)))
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

    public class Subject<T>
    {
        private readonly IList<Action<T>> observers = new List<Action<T>>();

        public IDisposable Subscribe(Action<T> observer)
        {
            observers.Add(observer);

            return new Disposable(() => observers.Remove(observer));
        }

        public void OnNext(T result)
        {
            foreach (var observer in observers)
            {
                observer(result);
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
}
