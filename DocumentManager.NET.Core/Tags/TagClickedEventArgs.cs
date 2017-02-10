using System;

namespace DocumentManager.NET.Core.Tags
{
    public class TagClickedEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// The _item
        /// </summary>
        private TagItem _item;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        public TagItem Item
        {
            get { return _item; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TagClickedEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public TagClickedEventArgs(TagItem item)
        {
            _item = item;
        }

        #endregion
    }
}
