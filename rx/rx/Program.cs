﻿using System;
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
            using (subject.Subscribe(new AnonymousBobserver<string>(s => Console.WriteLine(s), 
                () => Console.WriteLine("Done"),
                e => Console.WriteLine(e))))
            {
                var wc = new WebClient();
                var task = wc.DownloadStringTaskAsync("http://www.googleasdasdsad.com/robots.txt");
                task.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        subject.OnError(t.Exception);
                    else
                    {
                        subject.OnNext(t.Result);
                        subject.OnCompleted();
                    }
                });

                // Wait for the async call
                Console.ReadLine();
            }
        }
    }

    public class Bobservable
    {
        public static IBobservable<T> FromTask<T>()
        {

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
