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
}
