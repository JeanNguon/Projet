using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Jounce.Core.Application;
using Proteca.Silverlight.LogServiceReference;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Proteca.Silverlight.Services.Contracts;

namespace Proteca.Silverlight.Helpers
{
    /// <summary>
    /// Permet de logguer directement sur le serveur via l'appel d'un service asynchrone
    /// </summary>
#if (!DEBUG)
    [Export(typeof(ILogger))]
#endif
    public class CustomLogger : ILogger
    {
        public LogSeverity MinLogSeverity { get; set; }

        [Import]
        public IConfigurator ServiceConfigurator { get; set; }

        private LogServiceClient _logClient;
        private LogServiceClient LogClient
        {
            get
            {
                if (_logClient == null)
                {
                    Uri serviceUri = ServiceConfigurator.GetServiceAdress("Services/LogService.svc");
                    if (serviceUri.IsAbsoluteUri)
	                {
                        if (serviceUri.Scheme == "https")
                        {
                            _logClient = new LogServiceClient("BasicHttpsBinding_ILogService", serviceUri.ToString());
                        }
                        else
                        {
                            _logClient = new LogServiceClient("BasicHttpBinding_ILogService", serviceUri.ToString());
                        }
	                }
                    else
                    {
                        _logClient = new LogServiceClient("BasicHttpBinding_ILogService");
                    }
                }
                return _logClient;
            }
        }

        public void SetSeverity(LogSeverity minimumLevel)
        {
            MinLogSeverity = minimumLevel;
        }

        /// <summary>
        ///     Log a message
        /// </summary>
        /// <param name="severity">The severity</param>
        /// <param name="source">The source</param>
        /// <param name="message"></param>
        public void Log(LogSeverity severity, string source, string message)
        {
            if ((int)severity < (int)MinLogSeverity)
            {
                return;
            }
            LogClient.LogAsync((LogLevel)severity, source, message);
        }

        /// <summary>
        ///     Log with an exception
        /// </summary>
        /// <param name="severity">The severity</param>
        /// <param name="source">The source</param>
        /// <param name="exception">The exception</param>
        public void Log(LogSeverity severity, string source, Exception exception)
        {
            if ((int)severity < (int)MinLogSeverity)
            {
                return;
            }

            var sb = new StringBuilder();
            if (exception != null)
            {
                sb.Append(exception);

                var ex = exception.InnerException;

                while (ex != null)
                {
                    sb.AppendFormat("{0}{1}", Environment.NewLine, ex);
                    ex = ex.InnerException;
                } 
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
