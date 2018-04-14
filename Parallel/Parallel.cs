using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Parallel
{
    public static class Parallel
    {
        /// <summary>
        /// Executes a for loop in which iterations may run in parallel.
        /// </summary>
        /// <param name="fromInclusive">The start index</param>
        /// <param name="toExclusive">The end index</param>
        /// <param name="body">The delegate that is invoked once per iteration</param>
        public static void ParallelFor(int fromInclusive, int toExclusive, Action<int> body)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }

            if (toExclusive <= fromInclusive)
            {
                return;
            }

            var locker = new object();

            for (int i = fromInclusive; i < toExclusive; i++)
            {
                int local = i;
                Task.Run(() => body(local));
            }
        }


        /// <summary>
        /// Executes a foreach operation in which iterations may run in parallel.
        /// </summary>
        /// <typeparam name="TSource">Generic type</typeparam>
        /// <param name="source">An enumerable data source</param>
        /// <param name="body">The delegate that is invoked once per iteration</param>
        public static void ParallelForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (body == null)
            {
                throw new ArgumentNullException("body");
            }
            
            foreach (var item in source.ToList())
            {
                var local = (TSource)item;

                Task.Run(() => body(local));
            }
        }


        /// <summary>
        /// Executes a foreach operation in which iterations may run in parallel.
        /// </summary>
        /// <typeparam name="TSource"> Generic type</typeparam>
        /// <param name="source">An enumerable data source</param>
        /// <param name="parallelOptions">Instance that configures the behavior of this operation</param>
        /// <param name="body">The delegate that is invoked once per iteration</param>
        public static void ParallelForEachWithOptions<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Action<TSource> body)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (parallelOptions == null)
            {
                throw new ArgumentNullException("parallelOptions");
            }

            if (body == null)
            {
                throw new ArgumentNullException("body");
            }

            var loker = new Object();
            int maxCount = parallelOptions.MaxDegreeOfParallelism;
            int curCount = 0;
           
            int free = 0;

            var locker1 = new Object();
            var locker2 = new Object();
            var tasks = new Task[maxCount];

            foreach (var item in source.ToList())
            {
                var local = item;

                lock (locker1)
                {
                    if (curCount < maxCount)
                    {
                        tasks[free] = new Task(() => body(local));
                        tasks[free].Start();
                        free++;
                        curCount++;
                    }
                }


                if (curCount >= maxCount)
                {
                    lock (locker2)
                    {
                        free = Task.WaitAny(tasks);
                        curCount--;
                    }

                }
            }
            Task.WaitAll();

        }
    }
}
