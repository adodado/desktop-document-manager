using System;
using DocumentManager.NET.Core;

namespace DocumentManager.NET.Logging
{
    public class LogItem : ObjectBase
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>The details.</value>
        public string Details { get; set; }
        
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }
        
        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        /// <value>The origin.</value>
        public string Origin { get; set; }
        
        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>The severity.</value>
        public Severity Severity { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        public DateTime Timestamp { get; set; }

        #endregion
    }
}
