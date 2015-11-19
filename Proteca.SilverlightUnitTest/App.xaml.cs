using System;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using System.Windows;
using System.Windows.Controls;
using Jounce.Core.View;
using Jounce.Core.Event;
using System.ComponentModel.Composition;
using Jounce.Core.Application;
using Microsoft.Silverlight.Testing;
using Jounce.Framework.Services;

namespace Proteca.SilverlightUnitTest
{
    public partial class App : Application , IEventSink<UnhandledExceptionEvent>  
    {

        [Import]
        public IEventAggregator EventAggregator { get; set; }

        /// <summary>
        /// Holds a list of all views and their <see cref="IExportAsViewMetadata"/>
        /// </summary>
        [ImportMany(AllowRecomposition = true)]
        public Lazy<UserControl, IExportAsViewMetadata>[] Views { get; set; }

        public App()
        {
            InitializeComponent();

            Startup += (o, e) =>
            {
                RootVisual = UnitTestSystem.CreateTestPage();
            };
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            RootVisual = UnitTestSystem.CreateTestPage();
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }

        public void HandleEvent(UnhandledExceptionEvent publishedEvent)
        {
            //publishedEvent.UncaughtException.
            //Logger.Log(LogSeverity.Error, this.GetType().ToString(), publishedEvent.UncaughtException);
            //ErrorWindow.CreateNew(publishedEvent.UncaughtException);
        }
    }
}