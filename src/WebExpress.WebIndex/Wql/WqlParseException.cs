using System;

namespace WebExpress.WebIndex.Wql
{
    /// <summary>
    /// Represents an exception that is thrown when a WQL parsing error occurs.
    /// </summary>
    public class WqlParseException : Exception
    {
        /// <summary>
        /// Returns the token that caused the exception.
        /// </summary>
        public WqlToken Token { get; private set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="message">The massage.</param>
        /// <param name="token">The token that caused the exception.</param>
        public WqlParseException(string message, WqlToken token)
            : base(message)
        {
            Token = token;
        }
    }
}