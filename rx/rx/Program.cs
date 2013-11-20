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
            var wc = new WebClient();
            var task = wc.DownloadStringTaskAsync("http://www.google.com/robots.txt");
            task.ContinueWith(t => Console.WriteLine(t.Result));

            // Wait for the async call
            Console.ReadLine();
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
    }

    public class Disposable : IDisposable
    {
        private readonly Action action;

        public Disposable(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            action();
        }
    }
}
