using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;

namespace Proteca.Web.Services
{

    /// <summary>
    ///     Severity for the log message
    /// </summary>
    public enum LogLevel : short
    {
        /// <summary>
        /// All activities
        /// </summary>
        Verbose = 0,
        /// <summary>
        /// Only import information
        /// </summary>
        Information = 1,
        /// <summary>
        /// Warnings and worse
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Errors and worse
        /// </summary>
        Error = 3,
        /// <summary>
        /// Critical errors that compromise application execution
        /// </summary>
        Critical = 4
    }

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ILogService" in both code and config file together.
    [ServiceContract]
    public interface ILogService
    {
        [OperationContract]
        void Log(LogLevel severity, string source, string message);
    }
}
