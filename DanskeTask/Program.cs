using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanskeTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var http = new HttpHelper();
            
            Console.WriteLine("Enter one or more links (seperated by whitespace), to get their index pages: ");
            Console.WriteLine("Example: http://www.delfi.lt http://www.15min.lt ...");
            //I assume that user will write the input as I want so I don't do any input checking...
            var linksString = Console.ReadLine();

            var linkList = linksString?.Split(' ');
            var tasks = linkList.Select(t => Task<string>.Factory.StartNew(() => http.HttpGet(t))).ToArray();
            Task.WaitAll(tasks);

            var results = tasks.Select(task => task.Result).ToList();

            foreach (var result in results) //Not sure how the input should look like, so I just print it all out in the console...
            {
                Console.WriteLine(result);
                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}
