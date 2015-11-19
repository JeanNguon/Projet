using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Jounce.Core.Application;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Framework.Services;
using Microsoft.Silverlight.Testing;
using Microsoft.Silverlight.Testing.UnitTesting.Metadata.VisualStudio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proteca.Silverlight.Helpers;
using System.Threading;

namespace Proteca.SilverlightUnitTest
{
    [TestClass]
    public class Tests : SilverlightTest, IPartImportsSatisfiedNotification
    {
        protected static ApplicationService applicationService = null;
        protected static bool IsRunning = false;

        public ILogger defaultLogger = new FakeLogger();

        private static List<string> testedVm = new List<string>();
        private static List<string> checkedVm = new List<string>();
        private static int testCount = 0;

        public Tests()
        {
            if (applicationService == null)
            {
                Console.WriteLine("Initialisation des Tests");

                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source =
                        new Uri(
                        "/Proteca.SilverlightUnitTest;component/Assets/Styles.xaml",
                        UriKind.RelativeOrAbsolute)
                });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source =
                        new Uri(
                        "/Proteca.SilverlightUnitTest;component/Assets/GridView.xaml",
                        UriKind.RelativeOrAbsolute)
                });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source =
                        new Uri(
                        "/Proteca.SilverlightUnitTest;component/Assets/Boutons.xaml",
                        UriKind.RelativeOrAbsolute)
                });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source =
                        new Uri(
                        "/Proteca.SilverlightUnitTest;component/Assets/TileView.xaml",
                        UriKind.RelativeOrAbsolute)
                });
                ResourceDictionary dic = new ResourceDictionary();
                dic.Add("ApplicationResources", new Proteca.Silverlight.ApplicationResources());
                Application.Current.Resources.MergedDictionaries.Add(dic);

                if (Application.Current.Resources.Any(r => r.Key.ToString() == "ServiceHostAdress"))
                {
                    Application.Current.Resources.Remove("ServiceHostAdress");
                }
                Application.Current.Resources.Add("ServiceHostAdress", "http://verd174:90/");
                if (!Application.Current.Resources.Any(r => r.Key.ToString() == "DebugLogin"))
                {
                    Application.Current.Resources.Add("DebugLogin", "n.cossard");
                }

                if (applicationService == null)
                {
                    applicationService = new ApplicationService();
                    applicationService.StartService(null);
                }
            }
        }

        [TestMethod]
        [Asynchronous]
        public void TestAllViewModels()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                Console.WriteLine("Debut TestAllViewModels");
                string currentViewName = null;
                string viewName = null;
               
                object lockObj = new object();
                foreach (var route in applicationService.Router.ViewModelRouter.RouteList.Where(r => !r.ViewType.Contains("Expander")))
                {
                    if (!testedVm.Contains(route.ViewModelType))
                    {
                        testedVm.Add(route.ViewModelType);
                        currentViewName = null;
                        viewName = route.ViewType;
                        var vmType = applicationService.Router.ViewModelRouter.ResolveViewModel(route.ViewModelType);
                        if (vmType != null)
                        {
                            var resolveVm = applicationService.Router.ViewModelRouter.GetType().GetMethods().FirstOrDefault(m => m.Name == "ResolveViewModel" && m.IsGenericMethod && m.GetParameters().Count() == 2);
                            if (resolveVm != null)
                            {
                                var genericResolveVm = resolveVm.MakeGenericMethod(new Type[1] { vmType.GetType() });
                                if (genericResolveVm != null)
                                {
                                    var vm = genericResolveVm.Invoke(applicationService.Router.ViewModelRouter, new object[2] { false, route.ViewModelType });
                                    var userCanRead = vm.GetType().GetProperty("UserCanRead");
                                    if (userCanRead != null && (bool)userCanRead.GetValue(vm, null))
                                    {
                                        if (vm != null)
                                        {

                                            var eventInfo = vm.GetType().GetEvent("OnAllServicesLoaded");
                                            if (eventInfo != null)
                                            {
                                                eventInfo.AddEventHandler(vm, new EventHandler((oo, ee) =>
                                                    {
                                                        checkedVm.Add(oo.GetType().FullName);
                                                        Console.WriteLine("2 - " + vm.GetType().FullName);
                                                        var success = !((FakeLogger)defaultLogger).Errors.Any(e => e.Key == vm.GetType().FullName);
                                                        Console.WriteLine(string.Format("Test {0} : {1}", vm.GetType().FullName, success ? "OK" : "KO"));
                                                        Assert.IsTrue(success);
                                                        lock(lockObj)
                                                        {
                                                            testCount --;
                                                        }
                                                        Console.WriteLine("testCount : " + testCount);
                                                    }));

                                                Console.WriteLine("1 - " + vm.GetType().FullName);
                                              
                                                applicationService.EventAggregator.Publish(new ViewNavigationArgs(viewName));
                                                lock (lockObj)
                                                {
                                                    testCount++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            EnqueueConditional(() => testCount == 0);
            EnqueueTestComplete();
        }


        public override void HandleException(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        public void OnImportsSatisfied()
        {

        }
    }
}