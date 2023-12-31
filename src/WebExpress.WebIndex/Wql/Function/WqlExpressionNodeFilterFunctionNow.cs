using System;
using WebExpress.WebIndex;

namespace WebExpress.WebIndex.Wql.Function
{
    /// <summary>
    /// Describes the function expression of a wql statement.
    /// Returns the current date and time.
    /// </summary>
    public class WqlExpressionNodeFilterFunctionNow<T> : WqlExpressionNodeFilterFunction<T> where T : IWebIndexItem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WqlExpressionNodeFilterFunctionNow()
            :base("now")
        {
        }

        /// <summary>
        /// Executes the function.
        /// </summary>
        /// <returns>The return value.</returns>
        public override object Execute()
        {
            return DateTime.Now;
        }
    }
}