using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parallel
{
    class Program
    {
        public static void print(int x)
        {
            //int n = 1000;
            //while (--n > 0) ;
            Console.WriteLine(x);
        }
        static void Main(string[] args)
        {
            var list = new List<int>();
            ParallelOptions p = new ParallelOptions();
            p.MaxDegreeOfParallelism = 16;

            Parallel.ParallelFor(0, 15000, x => list.Add(x));
            Parallel.ParallelForEachWithOptions(list, p, print);

            //Parallel.ParallelForEach(list, x => print(x));

            //var list = new List<int>();
            //System.Threading.Tasks.Parallel.For(0, 12350, x => list.Add(x));
           
           // System.Threading.Tasks.Parallel.ForEach(list, p, x => Console.WriteLine(x));
            Console.Read();
        }
    }
}
