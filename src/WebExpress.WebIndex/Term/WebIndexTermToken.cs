namespace WebExpress.WebIndex.Term
{
    public class WebIndexTermToken
    {
        /// <summary>
        /// Returns the position of the token in the input value.
        /// </summary>
        public uint Position { get; internal set; }

        /// <summary>
        /// Returns the token value.
        /// </summary>
        public string Value { get; internal set; }

        public override string ToString()
        {
            return $"{Value}:{Position}";
        }
    }
}
