namespace WebExpress.WebIndex.Term
{
    public class IndexTermToken
    {
        /// <summary>
        /// Returns the position of the token in the input value.
        /// </summary>
        public uint Position { get; internal set; }

        /// <summary>
        /// Returns the token value.
        /// </summary>
        public string Value { get; internal set; }

        /// <summary>
        /// Returns the hash code.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            // Combine the hash codes of Name and Position
            return Value.GetHashCode() ^ Position.GetHashCode();
        }

        /// <summary>
        /// Comparison with another object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            var item = obj as IndexTermToken;
            return item != null && Value == item.Value && Position == item.Position;
        }

        /// <summary>
        /// Convert the object into a string representation. 
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{Value}:{Position}";
        }
    }
}
