using System;
using System.Diagnostics;

namespace WebExpress.WebIndex.Utility
{
    /// <summary>
    /// Represents a aspect for the profiling.
    /// </summary>
    public class ProfilingAspect : IDisposable
    {
        /// <summary>
        /// The stop watch.
        /// </summary>
        private Stopwatch _stopwatch;
        
        /// <summary>
        /// The naome of the method.
        /// </summary>
        private string _methodName;

        /// <summary>
        /// The naome of the method.
        /// </summary>
        private int _lineNumber;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lineNumber">The name of the method.</param>
        /// <param name="methodName">The line number of the method call.</param>
        public ProfilingAspect(string methodName, int lineNumber)
        {
            _methodName = methodName;
            _lineNumber = lineNumber;   
            _stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            _stopwatch.Stop();

            Profiling.Add(_methodName, _lineNumber, _stopwatch.ElapsedTicks);
        }
    }

}