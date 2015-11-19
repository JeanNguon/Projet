using System;
using System.Linq;
using System.IO;
using Ionic.Zip;
using System.Text;
using System.Windows.Controls;
using Proteca.Silverlight.Services;
using Jounce.Core.Event;
using Jounce.Framework;
using Proteca.Silverlight.Services.Contracts;
using System.Windows;
using Offline;
using System.Collections.Generic;

namespace Proteca.Silverlight.Helpers
{
    /// <summary>
    /// GUI elements to save and load the data to and from a file or isoStore
    /// Carefull : call it with the intention of the user
    /// </summary>
    public static class LoadSaveHelper
    {
        private readonly static string _unzipperPass = "Grt!Pr0t3c@";

        private readonly static string _fileName = "saveProtOn";

        private readonly static string _fileEnd = ".json";

        //private static Dictionary<string, string> _types = new Dictionary<string, int>(){{"ProtOn", "pon"},{"All", "*"}};

        /// <summary>
        /// Load the file and renavigate into the the app to refresh it (with GUI)
        /// </summary>
        /// <param name="synchronizationService"></param>
        /// <param name="eventAggregator"></param>
        /// <param name="navigationService"></param>
        public static void LoadFromFile(SynchronizationService synchronizationService, IEventAggregator eventAggregator, NavigationService navigationService)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "ProtOn (*.pon)|*.pon";

            bool? userClickedOK = dlg.ShowDialog();
            if (userClickedOK == true)
            {
                string data = String.Empty;

                using (ZipFile zip = ZipFile.Read(dlg.File.OpenRead()))
                {
                    if (zip.Any(f => f.FileName.EndsWith(_fileEnd)))
                    {
                        zip.Password = _unzipperPass;
                        using (Stream fileStream = zip.First(f => f.FileName.EndsWith(_fileEnd)).OpenReader())
                        {
                            using (StreamReader reader = new StreamReader(fileStream))
                            {
                                string lineOfData = String.Empty;
                                while ((lineOfData = reader.ReadLine()) != null)
                                    data += lineOfData;

                                OfflineStorage.SaveToIsoStore(data);

                                synchronizationService.ImporterContext(data);
                                if (synchronizationService.ImportedContextChanged)
                                    OfflineStorage.SaveToIsoStore(synchronizationService.domainContext);

                                RefreshContext(synchronizationService, eventAggregator, navigationService);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Fichier erroné !", "Le fichier sélectionné est vide.", MessageBoxButton.OK);
                    }
                }
            }
        }

        /// <summary>
        /// Reload the data existing in the IsoStorage and refresh the page
        /// </summary>
        /// <param name="synchronizationService"></param>
        /// <param name="eventAggregator"></param>
        /// <param name="navigationService"></param>
        public static void ReloadFromIsoStore(SynchronizationService synchronizationService, IEventAggregator eventAggregator, NavigationService navigationService)
        {
            if (synchronizationService.domainContext.RestoreFromIsoStore())
            {
                RefreshContext(synchronizationService, eventAggregator, navigationService);
            }
            else
            {
                MessageBox.Show("Pas de fichier dans le cache Silverlight.", "Fichier inexistant", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Reload the page by navigating and setting the tournée label
        /// </summary>
        /// <param name="synchronizationService"></param>
        /// <param name="eventAggregator"></param>
        /// <param name="navigationService"></param>
        private static void RefreshContext(SynchronizationService synchronizationService, IEventAggregator eventAggregator, NavigationService navigationService)
        {
            eventAggregator.Publish("FilAriane".AsViewNavigationArgs().AddNamedParameter("Title", synchronizationService.LibelleTournee));
            Refresh(navigationService);
        }

        /// <summary>
        /// Reload the screen to the first element loaded
        /// </summary>
        /// <param name="navigationService"></param>
        public static void Refresh(NavigationService navigationService)
        {
            navigationService.DesactivateCurrentView();
            navigationService.Navigate(0, true);
        }

        /// <summary>
        /// Save the data to a file (with GUI)
        /// </summary>
        public static void SaveToFile()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".pon";
            dialog.Filter = string.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", "pon", "ProtOn");
            dialog.FilterIndex = 1;

            bool? dialogResult = dialog.ShowDialog();
            if (dialogResult == true)
            {
                string jsonData = Offline.OfflineStorage.ExtractIsoStore();

                if (!String.IsNullOrEmpty(jsonData))
                {
                    using (Stream filestream = dialog.OpenFile())
                    {
                        using (ZipFile zip = new ZipFile(Encoding.UTF8))
                        {
                            zip.Password = _unzipperPass;

                            // Fichier de donnée
                            zip.AddEntry(_fileName + _fileEnd, jsonData);

                            zip.Save(filestream);
                        }
                    }
                }
            }
        }
    }
}
