using System.Globalization;

namespace WebExpress.WebIndex.Studio.Model
{
    public class ViewModel
    {
        public IndexManager IndexManager { get; } = new IndexManager();

        public Project Project { get; } = new Project();

        public IEnumerable<Object> Objects => Project.Objects;

        public ViewModel()
        {
            Project.Name = "Test";

            var obj1 = new Object() { Name = "Object_1" };

            obj1.Attributes.Add(new Attribute() { Name = "Name", Type = AttributeType.Text });
            obj1.Attributes.Add(new Attribute() { Name = "Attribute_2", Type = AttributeType.Double });
            obj1.Attributes.Add(new Attribute() { Name = "Attribute_3", Type = AttributeType.Bool });
            obj1.Attributes.Add(new Attribute() { Name = "Attribute_4", Type = AttributeType.Int });

            Project.Objects.Add(obj1);
            Project.Objects.Add(new Object() { Name = "Object_2" });
            Project.Objects.Add(new Object() { Name = "Object_3" });

            var context = new IndexContext { IndexDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WebIndex Studio") };
            IndexManager.Initialization(context);

            var runtimeClass = obj1.BuildClass();
            IndexManager.Create(runtimeClass, CultureInfo.GetCultureInfo("en"), IndexType.Storage);

            //IndexManager.Clear(runtimeClass);

            if (!IndexManager.All(runtimeClass).Any())
            {
                var id = runtimeClass.GetProperty("Id");
                var name = runtimeClass.GetProperty("Name");
                var attribute2 = runtimeClass.GetProperty("Attribute_2");
                var attribute3 = runtimeClass.GetProperty("Attribute_3");
                var attribute4 = runtimeClass.GetProperty("Attribute_4");

                var instance = Activator.CreateInstance(runtimeClass);
                id.SetValue(instance, Guid.NewGuid(), null);
                name.SetValue(instance, "Hello, World!", null);
                attribute2.SetValue(instance, 1293.76, null);
                attribute3.SetValue(instance, true, null);
                attribute4.SetValue(instance, 10, null);

                IndexManager.Insert(runtimeClass, instance);

                instance = Activator.CreateInstance(runtimeClass);
                id.SetValue(instance, Guid.NewGuid(), null);
                name.SetValue(instance, "Hello, Welt!", null);
                attribute2.SetValue(instance, 545.876, null);
                attribute3.SetValue(instance, false, null);
                attribute4.SetValue(instance, 5, null);

                IndexManager.Insert(runtimeClass, instance);
            }
        }
    }
}
