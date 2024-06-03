using System.Globalization;

namespace WebExpress.WebIndex.Studio.Converter
{
    /// <summary>
    /// Converts a null reference to a boolean value.
    /// </summary>
    public class NullToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Converts the value from null to a boolean.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Additional parameter for the converter.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A boolean indicating whether the input value is not null.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        /// <summary>
        /// Converts back the boolean to the original value type, which is not implemented.
        /// </summary>
        /// <param name="value">The value to be converted back.</param>
        /// <param name="targetType">The type to convert back to.</param>
        /// <param name="parameter">Additional parameter for the converter.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The original value type.</returns>
        /// <exception cref="NotImplementedException">Thrown when the method is not implemented.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
