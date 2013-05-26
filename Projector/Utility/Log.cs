//namespace Projector
//{
//    using System.Diagnostics;

//    internal static class Log
//    {
//        private static TraceSource traceSource;
//        private static bool        enabled;

//        public static TraceSource TraceSource
//        {
//            get { return traceSource ?? Concurrent.Ensure(ref traceSource, new TraceSource("Projector")); }
//        }

//        public static bool Enabled
//        {
//            get { return enabled; }
//            set { enabled = value; }
//        }

//        public static void Info(string text)
//        {
//            if (enabled)
//                TraceSource.TraceEvent(TraceEventType.Information, 0, text);
//        }

//        public static void Verbose(string text)
//        {
//            if (enabled)
//                TraceSource.TraceEvent(TraceEventType.Verbose, 0, text);
//        }
//    }
//}
