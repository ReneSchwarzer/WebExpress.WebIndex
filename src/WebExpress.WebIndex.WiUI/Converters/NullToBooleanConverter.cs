using System.Globalization;

namespace WebExpress.WebIndex.WiUI.Converters
{
    /// <summary>
    /// Converts a null value to false and any non-null value to true.
    /// </summary>
    public class NullToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value to a boolean. Returns false if the value is null, otherwise returns true.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture to be used in the converter.</param>
        /// <returns>False if the value is null, otherwise true.</returns>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null;
        }

        /// <summary>
        /// Converts a boolean value back to its original value. This method is not implemented and will throw an exception if called.
        /// </summary>
        /// <param name="value">The value to be converted back.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture to be used in the converter.</param>
        /// <returns>Throws a NotImplementedException.</returns>
        /// <exception cref="NotImplementedException">Thrown always as this method is not implemented.</exception>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
