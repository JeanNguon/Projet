namespace Proteca.Silverlight
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.DomainServices.Client.ApplicationServices;
    using System.Windows;
    using System.Windows.Controls;
    using Jounce.Core.View;
    using Jounce.Core.Event;
    using System.ComponentModel.Composition;
    using Jounce.Core.Application;
    using System.Globalization;
    using System.Threading;
    using Telerik.Windows.Controls;
    using System.Resources;
    using Proteca.Silverlight.Helpers;

    /// <summary>
    /// Main <see cref="Application"/> class.
    /// </summary>
    public partial class App : Application, IEventSink<UnhandledExceptionEvent>
    {
        [Import]
        public IEventAggregator EventAggregator { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        /// <summary>
        /// Creates a new <see cref="App"/> instance.
        /// </summary>
        public App()
        {
            LocalizationManager.Manager = new CustomLocalizationManager();

            InitializeComponent();
            // Create a WebContext and add it to the ApplicationLifetimeObjects collection.
            // This will then be available as WebContext.Current.
            WebContext webContext = new WebContext();
            this.ApplicationLifetimeObjects.Add(webContext);
            Startup += (o, e) =>
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");

                CompositionInitializer.SatisfyImports(this);
                EventAggregator.SubscribeOnDispatcher(this);
            };
        }

        public void HandleEvent(UnhandledExceptionEvent publishedEvent)
        {
            Logger.Log(LogSeverity.Error, this.GetType().ToString(), publishedEvent.UncaughtException);
        }


        private string _ClientContextUrl;
        public string ClientContextUrl
        {
            get
            {
                if (_ClientContextUrl == null)
                {
                    if (Resources.Contains("ClientContextUrl"))
                        _ClientContextUrl = Resources["ClientContextUrl"].ToString();
                }
                return _ClientContextUrl;
            }
        }
        private string _ServiceHostAdress;
        public string ServiceHostAdress
        {
            get
            {
                if (_ServiceHostAdress == null)
                {
                    if (Resources.Contains("ServiceHostAdress"))
                        _ServiceHostAdress = Resources["ServiceHostAdress"].ToString();
                }
                return _ServiceHostAdress;
            }
        }

    }
}