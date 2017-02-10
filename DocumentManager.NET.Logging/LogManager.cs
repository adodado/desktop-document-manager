using System;

namespace DocumentManager.NET.Logging
{
    public class LogManager : EventArgs
    {
        #region Public properties

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        public LogItem Item { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LogManager"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public LogManager(LogItem item)
        {
            Item = item;
        }

        #endregion
    }
}
