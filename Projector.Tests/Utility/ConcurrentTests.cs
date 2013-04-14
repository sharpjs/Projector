namespace Projector.Utility
{
    using System;
    using System.Threading;
    using NUnit.Framework;

    [TestFixture]
    public class ConcurrentTests
    {
        private const  int    TaskCount = 16;
        private static object sharedLock;
        private static int    sharedLocation; 

        [Test]
        public void Ensure()
        {
            var location = null as object;
            var results  = new object[TaskCount];

            GuaranteedParallel(i =>
            {
                results[i] = Concurrent.Ensure(ref location, new object());
            });

            Assert.That(location, Is.Not.Null);
            Assert.That(results,  Is.All.SameAs(location));
        }

        [Test]
        public void Lock_Unlock()
        {
            GuaranteedParallel(UseLock);
        }

        private static void UseLock(int i)
        {
            var token = Concurrent.Lock(ref sharedLock);
            try
            {
                sharedLocation = i;
                Thread.Yield();
                Thread.MemoryBarrier();
                Assert.That(sharedLocation, Is.EqualTo(i));
            }
            finally
            {
                Concurrent.Unlock(ref sharedLock, token);
            }
        }

        private static void GuaranteedParallel(Action<int> action)
        {
            // RATIONALE: Parallel.For does NOT guarantee multithreading.
            if (action == null)
                throw Error.ArgumentNull("action");

            var threads = new Thread[TaskCount];
            int i;

            for (i = 0; i < threads.Length; i++)
            {
                var index = i; // avoid "closure over loop variable" bug
                threads[i] = new Thread(() => action(index))
                {
                    IsBackground = true
                };
            }

            for (i = 0; i < threads.Length; i++)
                threads[i].Start();

            for (i = 0; i < threads.Length; i++)
                threads[i].Join();
        }
    }
}
