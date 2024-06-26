﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using WebExpress.WebIndex.WebAttribute;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Represents a index schema file.
    /// </summary>
    /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
    public class IndexStorageSchema<T> : IIndexSchema<T> where T : IIndexItem
    {
        /// <summary>
        /// Returns the file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Returns the index context.
        /// </summary>
        public IIndexContext Context { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexSchema"/> class.
        /// </summary>
        /// <param name="context">The index context.</param>
        public IndexStorageSchema(IIndexContext context)
        {
            Context = context;
            FileName = Path.Combine(Context.IndexDirectory, $"{typeof(T).Name}.ws");

            if (!File.Exists(FileName))
            {
                Write();
            }
        }

        /// <summary>
        /// Checks if the schema of the object has changed.
        /// </summary>
        /// <returns>
        /// True if the schema has changed, otherwise false.
        /// </returns>
        /// <remarks>
        /// This function compares the current schema of an object with the stored schema and returns true if there are changes, and false otherwise.
        /// </remarks>
        public bool HasSchemaChanged()
        {
            if (!File.Exists(FileName))
            {
                return true;
            }

            var currentSchema = GetSchema();
            var storedSchema = Read();

            var options = new JsonSerializerOptions { WriteIndented = true };
            var currentJson = JsonSerializer.Serialize(currentSchema, options);
            var storedJson = JsonSerializer.Serialize(storedSchema, options);

            return !currentJson.Equals(storedJson);
        }

        /// <summary>
        /// Migrates the schema if it has changed.
        /// </summary>
        public void Migrate()
        {
            if (HasSchemaChanged())
            {
                Write();
            }
        }

        /// <summary>
        /// Gets the schema of the object.
        /// </summary>
        /// <returns>
        /// The schema of the object as an anonymous type.
        /// </returns>
        private dynamic GetSchema()
        {
            var objectType = typeof(T);
            var fields = objectType.GetProperties().Select(x => new
            {
                x.Name,
                Type = GetType(x),
                Ignore = Attribute.IsDefined(x, typeof(IndexIgnoreAttribute)),
                Abstract = x.GetGetMethod().IsAbstract
            });
            var schema = new { objectType.Name, Fields = fields };

            return schema;
        }

        /// <summary>
        /// Returns the type.
        /// </summary>
        /// <param name="property">The property info.</param>
        /// <returns>The name of the type.</returns>
        private object GetType(PropertyInfo property)
        {
            if (property.PropertyType.IsPrimitive)
            {
                return property.PropertyType.Name;
            }
            else if (property.PropertyType == typeof(string))
            {
                return property.PropertyType.Name;
            }
            else if (property.PropertyType == typeof(Guid))
            {
                return property.PropertyType.Name;
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                return property.PropertyType.Name;
            }

            return "Object";

            //return new
            //{ 
            //Name = property.PropertyType.Name, 
            //Fields = property.PropertyType.GetProperties().Select(x => new 
            //{ 
            //    Name = x.Name, 
            //    Type = GetType(x), 
            //    Ignore = Attribute.IsDefined(x, typeof(IndexIgnoreAttribute))
            //})
            //};
        }

        /// <summary>
        /// Writes the schema of the object to the storage medium.
        /// </summary>
        private void Write()
        {
            var schema = GetSchema();
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(schema, options);

            File.WriteAllText(FileName, json);
        }

        /// <summary>
        /// Reads the object schema from the storage medium.
        /// </summary>
        /// /// <returns>
        /// The deserialized schema as an dynamic object.
        /// </returns>
        private dynamic Read()
        {
            var json = File.ReadAllText(FileName);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var schema = JsonSerializer.Deserialize<dynamic>(json, options);

            return schema;
        }

        /// <summary>
        /// Delete this file from storage.
        /// </summary>
        public void Drop()
        {
            Dispose();

            File.Delete(FileName);
        }

        /// <summary>
        /// Is called to free up resources.
        /// </summary>
        public virtual void Dispose()
        {

        }
    }
}