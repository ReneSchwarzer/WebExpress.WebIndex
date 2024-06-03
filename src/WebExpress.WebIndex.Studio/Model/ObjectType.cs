using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;
using WebExpress.WebIndex.WebAttribute;

namespace WebExpress.WebIndex.Studio.Model
{
    /// <summary>
    /// Represents an object within the WebExpress.WebIndex.Studio.
    /// </summary>
    public class ObjectType : BindableObject
    {
        private string _name;
        private string _description;
        private Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();

        /// <summary>
        /// Returns or sets the name of the object.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        /// <summary>
        /// Returns or sets the description of the object.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        /// <summary>
        /// Retuns a collection of attributes associated with the object.
        /// </summary>
        public ObservableCollection<Attribute> Attributes { get; } = [];

        /// <summary>
        /// Gets a collection of stored data objects.
        /// </summary>
        /// <remarks>
        /// This collection is populated with a maximum of 100 items from the index manager.
        /// </remarks>
        public ObservableCollection<object> Data => new(App.ViewModel.IndexManager.All(BuildClass()).Take(100));

        /// <summary>
        /// Dynamically builds a class based on the object's attributes.
        /// </summary>
        /// <returns>A Type representing the dynamically built class.</returns>
        public Type BuildClass()
        {
            if (_typeCache.TryGetValue(Name, out Type type))
            {
                return type;
            }

            var assemblyName = new AssemblyName("WebExpress.WebIndex.Studio.Model.Objects");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule($"Module_{Name}");

            var typeBuilder = moduleBuilder.DefineType(Name, TypeAttributes.Public, null, [typeof(IIndexItem)]);

            BildProperty(typeBuilder, "Id", typeof(Guid), true, true);

            foreach (var attribute in Attributes)
            {
                BildProperty(typeBuilder, attribute.Name, attribute.Type.ToType(), false, false);
            }

            var runtimeClass = typeBuilder.CreateType();
            _typeCache.Add(Name, runtimeClass);

            return runtimeClass;
        }

        /// <summary>
        /// Builds a property for the dynamically created class.
        /// </summary>
        /// <param name="typeBuilder">The builder for the class type.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="indexIgnore">Indicates whether the property should be ignored by the index.</param>
        /// <param name="virtual">Indicates whether the property should be virtual.</param>
        /// <returns>A PropertyBuilder for the created property.</returns>
        private PropertyBuilder BildProperty(TypeBuilder typeBuilder, string name, Type type, bool indexIgnore, bool @virtual)
        {
            var fieldBuilder = typeBuilder.DefineField($"_{name.ToLower()}", type, FieldAttributes.Private);
            var propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.HasDefault, type, Type.EmptyTypes);

            if (indexIgnore)
            {
                var attrCtorParams = Array.Empty<Type>();
                var attrCtorInfo = typeof(IndexIgnoreAttribute).GetConstructor(attrCtorParams);
                var attrBuilder = new CustomAttributeBuilder(attrCtorInfo, []);
                propertyBuilder.SetCustomAttribute(attrBuilder);
            }

            var getMethodBuilder = typeBuilder.DefineMethod($"get_{name}", @virtual ? MethodAttributes.Public | MethodAttributes.Virtual : MethodAttributes.Public, type, Type.EmptyTypes);
            var getIlGenerator = getMethodBuilder.GetILGenerator();
            getIlGenerator.Emit(OpCodes.Ldarg_0);
            getIlGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            getIlGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getMethodBuilder);

            var setMethodBuilder = typeBuilder.DefineMethod($"set_{name}", @virtual ? MethodAttributes.Public | MethodAttributes.Virtual : MethodAttributes.Public, null, [type]);
            var setIlGenerator = setMethodBuilder.GetILGenerator();
            setIlGenerator.Emit(OpCodes.Ldarg_0);
            setIlGenerator.Emit(OpCodes.Ldarg_1);
            setIlGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            setIlGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(setMethodBuilder);

            return propertyBuilder;
        }
    }
}
