using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Projector.Utility
{
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

            Parallel.For(0, TaskCount, i =>
            {
                results[i] = Concurrent.Ensure(ref location, new object());
            });

            Assert.That(location, Is.Not.Null);
            Assert.That(results, Is.All.SameAs(location));
        }

        [Test]
        public void Lock_Unlock()
        {
            Parallel.For(0, TaskCount, UseLock);
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
    }
}
