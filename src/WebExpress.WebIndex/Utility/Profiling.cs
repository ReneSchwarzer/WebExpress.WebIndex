using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WebExpress.WebIndex.Utility
{
    /// <summary>
    /// Represents a utility class for profiling.
    /// </summary>
    public static class Profiling
    {
        /// <summary>
        /// The directory for recording the diagnostic data.
        /// id, name, line number, count, total ticks, min ticks, average ticks, max ticks
        /// </summary>
        private static readonly Dictionary<string, (int, string, int, int, long, long, long, long)> _profiling = [];

        /// <summary>
        /// Returns a guard to protect against concurrent access.
        /// </summary>
        internal static object _guard = new();

        private static int _callid = 0;

        /// <summary>
        /// Create a diagnostic aspect.
        /// </summary>
        /// <returns></returns>
        public static ProfilingAspect Diagnostic([CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            var name = $"{Path.GetFileNameWithoutExtension(filePath)}.{methodName}";

            lock (_guard)
            {
                if (_profiling.TryAdd(name + lineNumber, (_callid, name, lineNumber, 0, 0, 0, 0, 0)))
                {
                    _callid++;
                }

                return new ProfilingAspect(name, lineNumber);
            }
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="ticks">The number of ticks required.</param>
        public static void Add(string methodName, int lineNumber, long ticks)
        {
            lock (_guard)
            {
                if (_profiling.TryGetValue(methodName + lineNumber, out var result))
                {
                    result.Item4++;
                    result.Item5 += ticks;
                    result.Item6 = Math.Min(ticks, result.Item6);
                    result.Item7 = result.Item5 / result.Item4;
                    result.Item8 = Math.Max(ticks, result.Item8);

                    _profiling[methodName + lineNumber] = result;
                }
            }
        }

        /// <summary>
        /// Saves the diagnostic data in the file system.
        /// </summary>
        public static void Store()
        {
            var file = File.CreateText(Path.Combine(Environment.CurrentDirectory, "profiling.csv"));
            file.WriteLine("id;method;line number;number of samples;total ticks;min ticks;average ticks;max ticks;duration;");

            foreach(var profile in _profiling.Values.OrderBy(x => x.Item1))
            {
                file.WriteLine($"{profile.Item1};{profile.Item2};{profile.Item3};{profile.Item4:n0};{profile.Item5:n0};{profile.Item6:n0};{profile.Item7:n0};{profile.Item8:n0};{new TimeSpan(profile.Item5):hh\\:mm\\:ss\\:fff};");
            }

            file.Close();
        }
    }

}