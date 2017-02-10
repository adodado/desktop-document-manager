using System;

namespace DocumentManager.NET.Logging
{
    public interface ILoger
    {
        #region Events

        event EventHandler<LogManager> OnLogItemAdded;

        #endregion

        #region Methods of ILoger

        /// <summary>
        /// Debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Debug(string message);

        /// <summary>
        /// Debug exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void Debug(Exception exception);

        /// <summary>
        /// Error message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Error(string message);

        /// <summary>
        /// Error exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void Error(Exception exception);

        /// <summary>
        /// Fatal message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Fatal(string message);

        /// <summary>
        /// Fatal exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void Fatal(Exception exception);

        /// <summary>
        /// Information message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Information(string message);

        /// <summary>
        /// Information exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void Information(Exception exception);

        /// <summary>
        /// Warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Warning(string message);

        /// <summary>
        /// Warning exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void Warning(Exception exception);

        #endregion
    }
}
