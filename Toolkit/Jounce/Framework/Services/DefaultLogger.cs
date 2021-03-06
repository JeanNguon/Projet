﻿using System;
using System.Diagnostics;
using System.Text;
using Jounce.Core.Application;

namespace Jounce.Framework.Services
{
    /// <summary>
    ///     The default (debug) logger
    /// </summary>
    /// <remarks>
    /// This logger simply writes to the debugger. To override it, implement the <see cref="ILogger"/>
    /// interface and export your own logger.
    /// </remarks>
    public class DefaultLogger : ILogger
    {
        /// <summary>
        ///     Template for logged messages
        /// </summary>
        private const string TEMPLATE = "{0} {1} {2} :: {3}";

        /// <summary>
        /// The sevierty of the message
        /// </summary>
        private LogSeverity _severityLevel;

        /// <summary>
        /// Default constructor at the warning level
        /// </summary>
        public DefaultLogger()
            : this(LogSeverity.Warning)
        {

        }

        /// <summary>
        /// Constructor with a user-specified level passed
        /// </summary>
        /// <param name="minimumSeverity">The minimum level to log</param>
        public DefaultLogger(LogSeverity minimumSeverity)
        {
            _severityLevel = minimumSeverity;
        }

        /// <summary>
        ///     Sets the severity 
        /// </summary>
        /// <param name="minimumLevel">Minimum level</param>
        public void SetSeverity(LogSeverity minimumLevel)
        {
            _severityLevel = minimumLevel;
        }

        /// <summary>
        ///     Log with a message
        /// </summary>
        /// <param name="severity">The severity</param>
        /// <param name="source">The source</param>
        /// <param name="message">The message</param>
        public void Log(LogSeverity severity, string source, string message)
        {
            if (!Debugger.IsAttached || (int)severity < (int)_severityLevel)
            {
                return;
            }

            Debug.WriteLine(TEMPLATE, DateTime.Now, severity, source, message);
        }

        /// <summary>
        ///     Log with an exception
        /// </summary>
        /// <param name="severity">The severity</param>
        /// <param name="source">The source</param>
        /// <param name="exception">The exception</param>
        public void Log(LogSeverity severity, string source, Exception exception)
        {
            if (!Debugger.IsAttached || (int)severity < (int)_severityLevel)
            {
                return;
            }

            var sb = new StringBuilder();
            sb.Append(exception);

            Exception ex = null;
            if (exception != null)
            {
                ex = exception.InnerException;
            }

            while (ex != null)
            {
                sb.AppendFormat("{0}{1}", Environment.NewLine, ex);
                ex = ex.InnerException;
            }

            Log(severity, source, sb.ToString());
        }

        /// <summary>
        ///     Log with formatting
        /// </summary>
        /// <param name="severity">The severity</param>
        /// <param name="source">The source</param>
        /// <param name="messageTemplate">The message template</param>
        /// <param name="arguments">The lines to log</param>
        public void LogFormat(LogSeverity severity, string source, string messageTemplate, params object[] arguments)
        {
            Log(severity, source, string.Format(messageTemplate, arguments));
        }
    }
}
