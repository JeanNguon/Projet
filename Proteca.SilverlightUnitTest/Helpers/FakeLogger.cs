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
using System.Collections.Generic;

namespace Proteca.Silverlight.Helpers
{
    /// <summary>
    /// Permet de logguer directement sur le serveur via l'appel d'un service asynchrone
    /// </summary>
    [Export(typeof(ILogger))]
    public class FakeLogger : ILogger
    {
        public List<KeyValuePair<String, Exception>> Errors = new List<KeyValuePair<string, Exception>>();

        public void SetSeverity(LogSeverity minimumLevel)
        {

        }

        public void Log(LogSeverity severity, string source, string message)
        {
            Console.WriteLine(string.Format("Log : {0} - {1} - {2}", severity.ToString(), source, message));
            if (severity == LogSeverity.Error || severity == LogSeverity.Critical)
            {
                Errors.Add(new KeyValuePair<string, Exception>(source, new Exception(message)));
            }
        }

        public void Log(LogSeverity severity, string source, Exception exception)
        {
            Console.WriteLine(string.Format("Log : {0} - {1} - {2}", severity.ToString(), source, exception.ToString()));
            if (severity == LogSeverity.Error || severity == LogSeverity.Critical)
            {
                Errors.Add(new KeyValuePair<string, Exception>(source, exception));
            }
        }

        public void LogFormat(LogSeverity severity, string source, string messageTemplate, params object[] arguments)
        {
        }
    }
}
