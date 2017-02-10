using System;
using System.Diagnostics;

namespace DocumentManager.NET.Logging
{
    public class Loger : ILoger
    {
        #region Events

        /// <summary>
        /// Occurs when [on log item added].
        /// </summary>
        public event EventHandler<LogManager> OnLogItemAdded;

        #endregion

        #region Methods of Loger

        /// <summary>
        /// Adds the specified severity.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="message">The message.</param>
        /// <param name="details">The details.</param>
        private void Add(Severity severity, string message, string details = "")
        {
            var item = new LogItem
            {
                Severity = severity,
                Timestamp = DateTime.Now,
                Origin = GetStackTrace(5),
                Message = message,
                Details = details
            };

            if (OnLogItemAdded != null)
                OnLogItemAdded(this, new LogManager(item));
        }

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
#if DEBUG
            Add(Severity.Debug, message, string.Empty);
#endif
        }

        /// <summary>
        /// Debugs the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Debug(Exception exception)
        {
#if DEBUG
            Add(Severity.Debug, exception.Message, exception.ToString());
#endif
        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            Add(Severity.Error, message);
        }

        /// <summary>
        /// Errors the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Error(Exception exception)
        {
            Add(Severity.Error, exception.Message, exception.ToString());
        }

        /// <summary>
        /// Fatals the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Fatal(string message)
        {
            Add(Severity.Fatal, message);
        }

        /// <summary>
        /// Fatals the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Fatal(Exception exception)
        {
            Add(Severity.Fatal, exception.Message, exception.ToString());
        }

        /// <summary>
        /// Gets the stack trace.
        /// </summary>
        /// <param name="noOfLevels">The no of levels.</param>
        /// <returns>System.String.</returns>
        private string GetStackTrace(int noOfLevels)
        {
            var st = new StackTrace();
            var strClass = string.Empty;
            var strMethod = string.Empty;
            var strOrigin = string.Empty;
            var iFrameNo = 0;
            var iCount = 0;

            try
            {
                while (iFrameNo < st.FrameCount)
                {
                    var m = st.GetFrame(iFrameNo).GetMethod();
                    if (m.ReflectedType != null)
                    {
                        strClass = st.GetFrame(iFrameNo).GetMethod().ReflectedType.Name;
                        //Don't include calls in this class
                        if (strClass != "Logger")
                        {
                            strMethod = st.GetFrame(iFrameNo).GetMethod().Name;
                            if (strOrigin == string.Empty)
                                strOrigin = strClass + "." + strMethod;
                            else
                                strOrigin += ", " + strClass + "." + strMethod;
                            iCount++;
                        }
                    }

                    iFrameNo++;

                    if (iCount >= noOfLevels)
                        break;
                }
            }
            catch (Exception)
            {
            }

            return strOrigin;
        }

        /// <summary>
        /// Informations the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Information(string message)
        {
            Add(Severity.Information, message);
        }

        /// <summary>
        /// Informations the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Information(Exception exception)
        {
            Add(Severity.Information, exception.Message, exception.ToString());
        }

        /// <summary>
        /// Warnings the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warning(string message)
        {
            Add(Severity.Warning, message);
        }

        /// <summary>
        /// Warnings the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Warning(Exception exception)
        {
            Add(Severity.Warning, exception.Message, exception.ToString());
        }

        #endregion
    }
}
