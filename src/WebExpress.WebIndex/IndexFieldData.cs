using System;
using System.Linq.Expressions;
using System.Reflection;
using WebExpress.WebIndex.WebAttribute;

namespace WebExpress.WebIndex
{
    /// <summary>
    /// Represents the data for an index field.
    /// </summary>
    public class IndexFieldData
    {
        private Func<object, object> _cachedLambda { get; set; }

        /// <summary>
        /// Returns the name of the index field.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Returns the type of the index field.
        /// </summary>
        public Type Type { get; internal set; }

        /// <summary>
        /// Returns the PropertyInfo of the index field.
        /// </summary>
        public PropertyInfo PropertyInfo { get; internal set; }

        /// <summary>
        /// Returns a value indicating whether the index field is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if the index field is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled => PropertyInfo.GetCustomAttribute<IndexIgnoreAttribute>() == null;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexFieldData"/> class.
        /// </summary>
        public IndexFieldData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexFieldData"/> class with the specified property.
        /// </summary>
        /// <param name="property">The property information to initialize the index field 
        public IndexFieldData(PropertyInfo property)
        {
            Name = property?.Name;
            Type = property?.PropertyType;
            PropertyInfo = property;
        }

        /// <summary>
        /// Retrieves the value of a property from an object based on the specified field.
        /// </summary>
        /// <param name="item">The object from which to retrieve the property value.</param>
        /// <returns>The value of the specified property, or null if the property is not found.</returns>
        public object GetPropertyValue(object item)
        {
            if (item == null)
            {
                return null;
            }

            if (_cachedLambda == null)
            {
                var propertyNames = Name.Split('.');
                var parameter = Expression.Parameter(typeof(object), "item");

                var currentExpression = Expression.Convert(parameter, item.GetType()) as Expression;
                var currentType = item.GetType();

                foreach (var propertyName in propertyNames)
                {
                    if (currentType == null)
                    {
                        return null;
                    }

                    var property = currentType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                    if (property == null)
                    {
                        return null;
                    }

                    currentExpression = Expression.Condition
                    (
                        Expression.NotEqual(currentExpression, Expression.Constant(null, currentType)),
                        Expression.Convert(Expression.Property(Expression.Convert(currentExpression, currentType), property), typeof(object)),
                        Expression.Constant(null, typeof(object))
                    );

                    currentType = property.PropertyType;
                }

                var lambda = Expression.Lambda<Func<object, object>>
                (
                    Expression.Convert(currentExpression, typeof(object)),
                    parameter
                ).Compile();

                _cachedLambda = lambda;
            }

            return _cachedLambda(item);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {

            return $"Field: {Name}";

        }
    }
}
