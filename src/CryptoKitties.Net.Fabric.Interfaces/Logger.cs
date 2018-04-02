using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoKitties.Net.Fabric
{
    public static class Logger
    {
        public static void WriteToLog(TraceEventType eventType, string title, string message, params object[] args)
        { WriteToLog(eventType, title, default(IDictionary<string, object>), message, args); }
        public static void WriteToLog(TraceEventType eventType, string title, IDictionary<string, object> extendedProperties, string message, params object[] args)
        { WriteToLog(eventType, default(IEnumerable<string>), title, extendedProperties, message, args); }
        public static void WriteToLog(IEnumerable<string> categories, string title, string message, params object[] args)
        { WriteToLog(categories, title, default(IDictionary<string, object>), message, args); }
        public static void WriteToLog(IEnumerable<string> categories, string title, IDictionary<string, object> extendedProperties, string message, params object[] args)
        { WriteToLog(TraceEventType.Verbose, categories, title, extendedProperties, message, args); }
        public static void WriteToLog(TraceEventType eventType, IEnumerable<string> categories, string title, string message, params object[] args)
        { WriteToLog(eventType, categories, title, default(IDictionary<string, object>), message, args); }
        public static void WriteToLog(TraceEventType eventType, IEnumerable<string> categories, string title, IDictionary<string, object> extendedProperties, string message, params object[] args)
        {
            // Normalize Categories
            categories = categories ?? new[] { Categories.Verbose };
            // Normalize title
            title = title ?? "Log Message";
            // Normalize / Format Message
            args = args ?? new object[0];
            if (args.Length > 0)
            {
                message = string.Format(message, args);
            }
            // Then write to trace
            Trace.WriteLine(string.Format("Title: {0}\r\n{1}", title ?? "Log Message", message), string.Join(",", categories.ToArray()));
        }
        public static void WriteToLog(this Exception instance, string title = default(string), IDictionary<string, object> extendedProperties = default(IDictionary<string, object>))
        { WriteToLog(instance, default(IEnumerable<string>), title, extendedProperties); }
        public static void WriteToLog(this Exception instance, IEnumerable<string> categories, string title = default(string), IDictionary<string, object> extendedProperties = default(IDictionary<string, object>))
        {
            var cats = new List<string>(categories ?? new string[0]);
            if (!cats.Contains(Categories.Error))
                cats.Add(Categories.Error);
            if (title == null) title = instance.Message;
            WriteToLog(TraceEventType.Error, categories, title, extendedProperties, "{0}", instance);
        }
        /// <summary>
        /// The <see cref="Categories"/> class defines well-known log categories.
        /// </summary>
        public static class Categories
        {
            /// <summary>
            /// Information
            /// </summary>
            public const string Information = "Information";
            /// <summary>
            /// Application
            /// </summary>
            public const string Application = "Application";
            /// <summary>
            /// Error
            /// </summary>
            public const string Error = "Error";
            /// <summary>
            /// Verbose
            /// </summary>
            public const string Verbose = "Verbose";
            /// <summary>
            /// Trace
            /// </summary>
            public const string Trace = "Trace";
            /// <summary>
            /// Debug
            /// </summary>
            public const string Debug = "Debug";
        }
    }
}
