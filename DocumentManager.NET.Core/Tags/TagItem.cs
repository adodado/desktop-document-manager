namespace DocumentManager.NET.Core.Tags
{
    public class TagItem : ObjectBase
    {
        #region Fields

        /// <summary>
        /// The _name
        /// </summary>
        private string _name;
        /// <summary>
        /// The _value
        /// </summary>
        private int _value;
        /// <summary>
        /// The _index
        /// </summary>
        private int _index;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                RaisePropertyChanged(() => Index);
            }
        }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged(() => Value);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TagItem"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public TagItem(string name, int value)
        {
            Name = name;
            Value = value;
        }

        #endregion
    }
}
