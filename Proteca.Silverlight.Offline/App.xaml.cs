using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Proteca.Silverlight
{
    using System;
    using System.Runtime.Serialization;
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
    using Proteca.Web.Services;
    using Offline;
    using System.IO.IsolatedStorage;

    /// <summary>
    /// Main <see cref="Application"/> class.
    /// </summary> 
    public partial class App : Application, IEventSink<UnhandledExceptionEvent>
    {
        [Import]
        public IEventAggregator EventAggregator { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }


        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        /// <summary>
        /// Creates a new <see cref="App"/> instance.
        /// </summary>
        public App()
        {
            LocalizationManager.Manager = new CustomLocalizationManager();

            InitializeComponent();
            Startup += (o, e) =>
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");

                CompositionInitializer.SatisfyImports(this);
                EventAggregator.SubscribeOnDispatcher(this);

                //TODO placer ça dans un fichier de conf (utilise pour le debug)
                Logger.SetSeverity(LogSeverity.Verbose);
            };

            Exit += (o, e) =>
            {
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
