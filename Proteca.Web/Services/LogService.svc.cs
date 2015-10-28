using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using log4net;
using log4net.Config;

namespace Proteca.Web.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "LogService" in code, svc and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LogService : ILogService
    {
        static LogService()
        {
            XmlConfigurator.Configure(); 
        }

        public void Log(LogLevel severity, string source, string message)
        {
            if (String.IsNullOrEmpty(source))
            {
                source = "ProtecaV4";
            }
            //TODO log in Sharepoint ?
            ILog logger = LogManager.GetLogger(source);
            if (logger != null)
            {
                switch (severity)
                {
                    case LogLevel.Critical:
                        logger.Fatal(message);
                        break;
                    case LogLevel.Error:
                        logger.Error(message);
                        break;
                    case LogLevel.Warning:
                        logger.Warn(message);
                        break;
                    case LogLevel.Information:
                        logger.Info(message);
                        break;
                    case LogLevel.Verbose:
                        logger.Debug(message);
                        break;
                }
            }
        }
    }
}
