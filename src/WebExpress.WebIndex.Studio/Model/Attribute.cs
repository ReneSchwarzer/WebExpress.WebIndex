namespace WebExpress.WebIndex.Studio.Model
{
    public class Attribute : BindableObject
    {
        private string _name;
        private string _description;
        private AttributeType _type;

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
        /// Returns or sets the type of the attribute.
        /// </summary>
        public AttributeType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }
    }
}
