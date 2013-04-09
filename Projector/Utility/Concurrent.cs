namespace Projector
{
    using System.Threading;

    internal static class Concurrent
    {
        public static T Ensure<T>(ref T location, T value)
            where T : class
        {
            return Interlocked.CompareExchange(ref location, value, null) ?? value;
        }

        // For rarely-used locks
        public static object Lock(ref object location)
        {
            var token = new object();
            token = Interlocked.CompareExchange(ref location, token, null) ?? token;
            Monitor.Enter(token);
            return token;
        }

        // For rarely-used locks
        public static void Unlock(ref object location, object token)
        {
            location = null;
            Monitor.Exit(token);
        }
    }
}
