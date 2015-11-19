using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.ServiceModel.DomainServices.Client;
using Jounce.Core.Application;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Web.Models;
using Proteca.Web.Services;
using System.Collections.ObjectModel;
using Microsoft.SharePoint.Client;
using Proteca.Silverlight.Resources;
using System.Threading;
using Proteca.Silverlight.Models;
using System.IO;
using System.Windows;
using Proteca.Silverlight.Enums;

namespace Proteca.Silverlight.Services
{
    /// <summary>
    /// Service d'intéraction avec Sharepoint
    /// </summary>
    public class SharepointService
    {
        #region Properties

        private ClientContext _contextClientSharePoint;
        public ClientContext ContextClientSharePoint
        {
            get
            {
                if (_contextClientSharePoint == null && ServicesConfirugator != null)
                {
                    _contextClientSharePoint = ServicesConfirugator.GetClientContext();
                }
                return _contextClientSharePoint;
            }
        }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        [Import]
        public IConfigurator ServicesConfirugator { get; set; }

        /// <summary>
        /// Nom de la liste sharepoint
        /// </summary>
        protected String ListName
        {
            get
            {
                return "GED";
            }
        }

        public String DefaultFolder
        {
            get
            {
                return "/" + ListName;
            }
        }

        /// <summary>
        /// Liste sharepoint correspondant aux Documents
        /// </summary>
        public List SharepointList
        {
            get
            {
                return ContextClientSharePoint.Web.Lists.GetByTitle(ListName);
            }
        }

        /// <summary>
        /// L'utilisation d'un contexte synchronisé est nécessaire pour permettre l'utilisation des entités récupérées avec le thread utilisé par le modèle sharepoint dans le thread destiné à l'UI
        /// </summary>
        protected SynchronizationContext _syncCtxt = SynchronizationContext.Current;

        #endregion

        #region Sharepoint Methods

        /// <summary>
        /// Réinitialise le context sharepoint, doit être appelé avant chaque utilisation (à chaque début de méthode)
        /// </summary>
        protected void ResetContext()
        {
            if (this.ContextClientSharePoint != null)
            {
                this.ContextClientSharePoint.Dispose();
                this._contextClientSharePoint = null;
            }
        }

        /// <summary>
        /// Renomme le dossier sharepoint
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="folderName"></param>
        /// <param name="folderNewName"></param>
        /// <param name="completed"></param>
        public void RenameFolder(string relativePath, string folderName, string folderNewName, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            ResetContext();
            if (ContextClientSharePoint != null)
            {
                List list = SharepointList;

                // Recherche du dossier à renommer
                CamlQuery query = new CamlQuery();
                query.ViewXml = "<Query>" +
                                    "<Where>" +
                                        "<And>" +
                                            "<Eq>" +
                                                "<FieldRef Name=\"FSObjType\" />" +
                                                "<Value Type=\"Lookup\">1</Value>" +
                                             "</Eq>" +
                                              "<Eq>" +
                                                "<FieldRef Name=\"FileLeafRef\"/>" +
                                                "<Value Type=\"Text\">" + folderName + "</Value>" +
                                              "</Eq>" +
                                        "</And>" +
                                     "</Where>" +
                                "</Query>" +
                                "</View>";
                query.FolderServerRelativeUrl = relativePath;
                var folders = list.GetItems(query);
                ContextClientSharePoint.Load(list);
                ContextClientSharePoint.Load(list.Fields);
                ContextClientSharePoint.Load(folders, fs => fs.Include(fi => fi["Title"],
                    fi => fi.DisplayName,
                    fi => fi["FileDirRef"],
                    fi => fi["FileLeafRef"]));
                ContextClientSharePoint.ExecuteQueryAsync((o, e) =>
                {
                    var resFolders = folders.ToList();
                    int resCount = resFolders.Where(f => f.FieldValues["FileLeafRef"].ToString() == folderName).Count();
                    if (resCount == 1)
                    {
                        var folder = resFolders.Where(f => f.FieldValues["FileLeafRef"].ToString() == folderName).First();
                        folder["Title"] = folderNewName;
                        folder["FileLeafRef"] = folderNewName;
                        folder.Update();
                        ContextClientSharePoint.ExecuteQueryAsync((oo, ez) =>
                        {
                            _syncCtxt.Post(unused => completed(null), null);
                        },
                        (oo, ee) =>
                        {
                            _syncCtxt.Post(unused => completed(ee.Exception), null);
                        });
                    }
                    else
                    {
                        if (resCount < 1)
                        {
                            _syncCtxt.Post(unused => completed(new Exception(Resource.TypeDocument_SharepointFolderError)), null);
                        }
                        else if (resCount > 1)
                        {
                            _syncCtxt.Post(unused => completed(new Exception(Resource.TypeDocument_SharepointFolderExistError)), null);
                        }
                    }
                },
                    (o, e) =>
                    {
                        _syncCtxt.Post(unused => completed(e.Exception), null);
                    }
                );

            }
            else
            {
                var ex = new Exception(Resource.TypeDocument_SharepointContextError);
                Logger.Log(LogSeverity.Warning, this.GetType().FullName, ex);
                completed(ex);
            }
        }


        /// <summary>
        /// Supprime le dossier sharepoint
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="folderName"></param>
        /// <param name="completed"></param>
        public void DeleteFolder(string relativePath, string folderName, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            ResetContext();
            if (ContextClientSharePoint != null)
            {
                List list = SharepointList;

                // Recherche du dossier à supprimer
                CamlQuery query = new CamlQuery();
                query.ViewXml =
                               "<Query>" +
                                   "<Where>" +
                                       "<And>" +
                                           "<Eq>" +
                                               "<FieldRef Name=\"FSObjType\" />" +
                                               "<Value Type=\"Lookup\">1</Value>" +
                                            "</Eq>" +
                                             "<Eq>" +
                                               "<FieldRef Name=\"FileLeafRef\"/>" +
                                               "<Value Type=\"Text\">" + folderName + "</Value>" +
                                             "</Eq>" +
                                       "</And>" +
                                    "</Where>" +
                               "</Query>" +
                               "</View>";
                query.FolderServerRelativeUrl = relativePath;

                var folders = list.GetItems(query);

                ContextClientSharePoint.Load(list);
                ContextClientSharePoint.Load(folders);
                ContextClientSharePoint.Load(folders, fs => fs.Include(
                  fi => fi["FileDirRef"],
                  fi => fi["FileLeafRef"]));
                ContextClientSharePoint.ExecuteQueryAsync((o, e) =>
                {
                    var resFolders = folders.ToList();
                    if (resFolders.Where(f => f.FieldValues["FileLeafRef"].ToString() == folderName).Count() == 1)
                    {
                        var folder = resFolders.Where(f => f.FieldValues["FileLeafRef"].ToString() == folderName).First();
                        // Vérification qu'il n'y a pas de documents dans le dossier
                        if (int.Parse((string)folder.FieldValues["ItemChildCount"]) == 0)
                        {
                            folder.DeleteObject();
                            _syncCtxt.Post(unused => ContextClientSharePoint.ExecuteQueryAsync(
                            (oo, ee) =>
                            {
                                _syncCtxt.Post(unused2 => completed(null), null);
                            },
                            (oo, ee) =>
                            {
                                Logger.Log(LogSeverity.Error, this.GetType().FullName, ee.Exception);
                                _syncCtxt.Post(unused2 => completed(ee.Exception), null);
                            }), null);
                        }
                        else
                        {
                            _syncCtxt.Post(unused => completed(new Exception(Resource.TypeDocument_SharepointDeleteFolderError)), null);
                        }
                    }
                    else
                    {
                        _syncCtxt.Post(unused => completed(new Exception(Resource.TypeDocument_SharepointFolderError)), null);
                    }
                },
                (o, e) =>
                {
                    Logger.Log(LogSeverity.Error, this.GetType().FullName, e.Exception);
                    _syncCtxt.Post(unused => completed(e.Exception), null);
                });

            }
            else
            {
                var ex = new Exception(Resource.TypeDocument_SharepointContextError);
                Logger.Log(LogSeverity.Warning, this.GetType().FullName, ex);
                completed(ex);
            }
        }


        /// <summary>
        /// Créé le dossier sharepoint
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="folderName"></param>
        public void CreateFolder(string relativePath, string folderName, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            ResetContext();
            if (ContextClientSharePoint != null)
            {
                List list = SharepointList;

                ListItemCreationInformation newItem = new ListItemCreationInformation();
                newItem.UnderlyingObjectType = FileSystemObjectType.Folder;
                newItem.FolderUrl = relativePath;
                newItem.LeafName = folderName;

                ListItem item = list.AddItem(newItem);
                item.Update();
                ContextClientSharePoint.ExecuteQueryAsync(
                (o, e) =>
                {
                    _syncCtxt.Post(unused => completed(null), null);
                },
                (o, e) =>
                {
                    Logger.Log(LogSeverity.Error, this.GetType().FullName, e.Exception);
                    _syncCtxt.Post(unused => completed(e.Exception), null);
                });
            }
            else
            {
                var ex = new Exception(Resource.TypeDocument_SharepointContextError);
                Logger.Log(LogSeverity.Warning, this.GetType().FullName, ex);
                completed(ex);
            }
        }

        /// <summary>
        /// Ajoute un Document dans sharepoint
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <param name="metadatas"></param>
        /// <param name="completed"></param>
        public void CreateFile(string relativePath, string fileName, byte[] fileContent, Dictionary<string, object> metadatas, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            ResetContext();
            if (ContextClientSharePoint != null)
            {
                // Chargement de la liste
                ContextClientSharePoint.Load(SharepointList);
                ContextClientSharePoint.Load(SharepointList.RootFolder);
                ContextClientSharePoint.Load(SharepointList.RootFolder.Files);

                // Chargement des contentTypes
                ContentTypeCollection contentTypes = ContextClientSharePoint.Web.AvailableContentTypes;
                ContextClientSharePoint.Load(contentTypes);
                ContextClientSharePoint.ExecuteQueryAsync(
                (o, e) =>
                {
                    var ctid = from ct in contentTypes
                               where ct.Name == "Proteca Document"
                               select ct.Id;
                    FileCreationInformation file = new Microsoft.SharePoint.Client.FileCreationInformation();
                    file.Content = fileContent;
                    file.Overwrite = true;
                    file.Url = relativePath + "/" + fileName;
                    Microsoft.SharePoint.Client.File newFile = SharepointList.RootFolder.Files.Add(file);
                    ContextClientSharePoint.Load(newFile);
                    ListItem item = newFile.ListItemAllFields;
                    ContextClientSharePoint.Load(item);
                    foreach (var metadata in metadatas)
                    {
                        item[metadata.Key] = metadata.Value;
                    }
                    item["ContentTypeId"] = ctid;
                    item.Update();
                    ContextClientSharePoint.ExecuteQueryAsync(
                    (oo, ee) =>
                    {
                        _syncCtxt.Post(unused => completed(null), null);
                    },
                    (oo, ee) =>
                    {
                        Logger.Log(LogSeverity.Error, this.GetType().FullName, ee.Exception);
                        _syncCtxt.Post(unused => completed(ee.Exception), null);
                    });

                },
                (o, e) =>
                {
                    Logger.Log(LogSeverity.Error, this.GetType().FullName, e.Exception);
                    _syncCtxt.Post(unused => completed(e.Exception), null);
                }
                );
            }
            else
            {
                var ex = new Exception(Resource.TypeDocument_SharepointContextError);
                Logger.Log(LogSeverity.Warning, this.GetType().FullName, ex);
                completed(ex);
            }
        }

        public void UpdateFile(string relativePath, string fileName, Dictionary<string, object> metadatas, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            ResetContext();
            if (ContextClientSharePoint != null)
            {
                Folder folder = ContextClientSharePoint.Web.GetFolderByServerRelativeUrl(relativePath);
                var files = ContextClientSharePoint.LoadQuery(folder.Files.Include(f => f.Name));
                ContextClientSharePoint.ExecuteQueryAsync(
                (o, e) =>
                {
                    var resFiles = files.ToList();
                    var file = resFiles.Where(f => f.Name == fileName).FirstOrDefault();
                    ListItem item = file.ListItemAllFields;
                    ContextClientSharePoint.Load(item);
                    foreach (var metadata in metadatas)
                    {
                        item[metadata.Key] = metadata.Value;
                    }
                    // Mise à jour du document
                    item.Update();
                    ContextClientSharePoint.ExecuteQueryAsync(
                    (oo, ee) =>
                    {
                        _syncCtxt.Post(unused => completed(null), null);
                    },
                    (oo, ee) =>
                    {
                        Logger.Log(LogSeverity.Error, this.GetType().FullName, ee.Exception);
                        _syncCtxt.Post(unused => completed(ee.Exception), null);
                    });

                },
                (o, e) =>
                {
                    Logger.Log(LogSeverity.Error, this.GetType().FullName, e.Exception);
                    _syncCtxt.Post(unused => completed(e.Exception), null);
                }
                );
            }
            else
            {
                var ex = new Exception(Resource.TypeDocument_SharepointContextError);
                Logger.Log(LogSeverity.Warning, this.GetType().FullName, ex);
                completed(ex);
            }
        }

        /// <summary>
        /// Déplace un fichier dans sharepoint
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="newPath"></param>
        /// <param name="fileName"></param>
        /// <param name="completed"></param>
        protected void MoveFile(string relativePath, string newPath, string fileName, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            ResetContext();
            if (ContextClientSharePoint != null)
            {
                Folder folder = ContextClientSharePoint.Web.GetFolderByServerRelativeUrl(relativePath);
                var files = ContextClientSharePoint.LoadQuery(folder.Files.Include(f => f.Name));
                ContextClientSharePoint.ExecuteQueryAsync(
                (o, e) =>
                {
                    var resFiles = files.ToList();
                    var item = resFiles.Where(f => f.Name == fileName).FirstOrDefault();
                    if (item != null)
                    {
                        // Déplacement du document
                        item.MoveTo(newPath + "/" + fileName, MoveOperations.None);
                        ContextClientSharePoint.ExecuteQueryAsync(
                        (oo, ee) =>
                        {
                            _syncCtxt.Post(unused => completed(null), null);
                        },
                        (oo, ee) =>
                        {
                            Logger.Log(LogSeverity.Error, this.GetType().FullName, ee.Exception);
                            _syncCtxt.Post(unused => completed(ee.Exception), null);
                        });
                    }
                    else
                    {
                        _syncCtxt.Post(unused => completed(new Exception(string.Format("Le fichier {0} n'existe pas à cet emplacement : {1}.", fileName, relativePath))), null);
                    }
                },
                (o, e) =>
                {
                    Logger.Log(LogSeverity.Error, this.GetType().FullName, e.Exception);
                    _syncCtxt.Post(unused => completed(e.Exception), null);
                }
                );
            }
            else
            {
                var ex = new Exception(Resource.TypeDocument_SharepointContextError);
                Logger.Log(LogSeverity.Warning, this.GetType().FullName, ex);
                completed(ex);
            }
        }


        /// <summary>
        /// Supprime le document sharepoint
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="folderName"></param>
        public void DeleteFile(string relativePath, string fileName, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            ResetContext();
            if (ContextClientSharePoint != null)
            {
                List list = SharepointList;

                // Recherche du fichier
                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View Scope=\"FilesOnly\"> " +
                               "<Query>" +
                                   "<Where>" +
                                       "<And>" +
                                           "<Eq>" +
                                               "<FieldRef Name=\"FSObjType\" />" +
                                               "<Value Type=\"Integer\">0</Value>" +
                                            "</Eq>" +
                                             "<Eq>" +
                                               "<FieldRef Name=\"FileLeafRef\"/>" +
                                               "<Value Type=\"Text\">" + fileName + "</Value>" +
                                             "</Eq>" +
                                       "</And>" +
                                    "</Where>" +
                               "</Query>" +
                               "</View>";

                query.FolderServerRelativeUrl = relativePath;
                var files = list.GetItems(query);

                ContextClientSharePoint.Load(list);
                ContextClientSharePoint.Load(files);
                ContextClientSharePoint.Load(files, fs => fs.Include(
                  fi => fi["FileLeafRef"]));
                ContextClientSharePoint.ExecuteQueryAsync((o, e) =>
                {
                    var resFiles = files.ToList();
                    if (resFiles.Where(f => f.FieldValues["FileLeafRef"].ToString() == fileName).Count() == 1)
                    {
                        var file = resFiles.Where(f => f.FieldValues["FileLeafRef"].ToString() == fileName).First();
                        // Suppression du fichier
                        file.DeleteObject();
                        _syncCtxt.Post(unused => ContextClientSharePoint.ExecuteQueryAsync(
                        (oo, ee) =>
                        {
                            _syncCtxt.Post(unused2 => completed(null), null);
                        },
                        (oo, ee) =>
                        {
                            Logger.Log(LogSeverity.Error, this.GetType().FullName, ee.Exception);
                            _syncCtxt.Post(unused2 => completed(ee.Exception), null);
                        }), null);

                    }
                    else
                    {
                        _syncCtxt.Post(unused => completed(new Exception(Resource.TypeDocument_SharepointFileError)), null);
                    }
                },
                (o, e) =>
                {
                    Logger.Log(LogSeverity.Error, this.GetType().FullName, e.Exception);
                    _syncCtxt.Post(unused => completed(e.Exception), null);
                });

            }
            else
            {
                var ex = new Exception(Resource.TypeDocument_SharepointContextError);
                Logger.Log(LogSeverity.Warning, this.GetType().FullName, ex);
                completed(ex);
            }
        }


        /// <summary>
        /// Récupère tous les document d'un dossier sharepoint
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="folderName"></param>
        public void GetFiles(string cleOuvrage, string typeOuvrage, Action<Exception, List<Document>> completed, string relativePath = null)
        {
            List<Document> res = new List<Document>();
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            ResetContext();
            if (ContextClientSharePoint != null)
            {
                List list = SharepointList;

                // Recherche tous les fichiers correspondants à l'ouvrage
                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View Scope=\"RecursiveAll\"> " +
                               "<Query>" +
                                   "<Where>" +
                                   (!String.IsNullOrEmpty(typeOuvrage) || !String.IsNullOrEmpty(cleOuvrage) ? "<And>" : "") +
                                           "<Eq>" +
                                               "<FieldRef Name=\"FSObjType\" />" +
                                               "<Value Type=\"Integer\">0</Value>" +
                                            "</Eq>" +
                                           (!String.IsNullOrEmpty(typeOuvrage) && !String.IsNullOrEmpty(cleOuvrage) ?
                                             "<Eq>" +
                                               "<FieldRef Name=\"" + typeOuvrage + "\"/>" +
                                               "<Value Type=\"Text\">" + cleOuvrage + "</Value>" +
                                             "</Eq>"
                                             : !String.IsNullOrEmpty(typeOuvrage) ?
                                             "<IsNotNull>" + "<FieldRef Name=\"" + typeOuvrage + "\"/></IsNotNull>"
                                             : "") +
                                             (!String.IsNullOrEmpty(typeOuvrage) || !String.IsNullOrEmpty(cleOuvrage) ? "</And>" : "") +
                                    "</Where>" +
                               "</Query>" +
                               "</View>";
                if (!String.IsNullOrEmpty(relativePath))
                {
                    query.FolderServerRelativeUrl = relativePath;
                }
                var files = list.GetItems(query);

                ContextClientSharePoint.Load(list);
                ContextClientSharePoint.Load(files);
                ContextClientSharePoint.Load(files, fs => fs.Include(
                  fi => fi["FileLeafRef"],
                  fi => fi["ClePP"],
                  fi => fi["CleEquipement"],
                  fi => fi["ClePortion"],
                  fi => fi["CleEnsembleElectrique"],
                  fi => fi["Archive"],
                  fi => fi["NumEnregistrement"],
                  fi => fi["FileDirRef"],
                  fi => fi.File));
                ContextClientSharePoint.ExecuteQueryAsync((o, e) =>
                {
                    int cle = 0;
                    var resFiles = files.ToList();
                    CleOuvrage? fileTypeOuvrage = String.IsNullOrEmpty(typeOuvrage) ? (CleOuvrage?)null : (CleOuvrage?)Enum.Parse(typeof(CleOuvrage), typeOuvrage, true);
                    string fileCleOuvrage = null;
                    CleOuvrage? currentTypeOuvrage = null;
                    if (typeOuvrage == null || resFiles.Where(f => f.FieldValues[typeOuvrage].ToString() == cleOuvrage).Any())
                    {
                        foreach (var file in resFiles)
                        {
                            currentTypeOuvrage = null;
                            if (fileTypeOuvrage == null)
                            {
                                foreach (CleOuvrage type in Enum.GetValues(typeof(CleOuvrage)))
                                {
                                    fileCleOuvrage = string.Empty + file.FieldValues[type.ToString()];
                                    if (!String.IsNullOrEmpty(fileCleOuvrage) && !"0".Equals(fileCleOuvrage))
                                    {
                                        currentTypeOuvrage = type;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                currentTypeOuvrage = fileTypeOuvrage;
                                fileCleOuvrage = string.Empty + file.FieldValues[typeOuvrage];
                            }
                            // Initialisation de l'entité
                            Document doc = new Document(file.File.Name, (bool)file.FieldValues["Archive"], new TypeDocument(0, Path.GetFileName(Path.GetDirectoryName(file.File.ServerRelativeUrl)), file.File.ServerRelativeUrl));
                            doc.TypeOuvrage = currentTypeOuvrage.Value;
                            doc.NumeroVersion = "" + file.FieldValues["NumEnregistrement"];
                            doc.CleOuvrage = int.Parse(fileCleOuvrage);
                            doc.DateEnregistrement = file.File.TimeLastModified;
                            doc.DocumentUrl = new Uri(ContextClientSharePoint.Url + doc.Designation.ServerRelativeUrl + "/" + doc.Libelle, UriKind.Absolute);
                            doc.ItemId = file.Id;
                            doc.Cle = cle;
                            cle++;
                            res.Add(doc);
                        }
                    }
                    _syncCtxt.Post(unused => completed(null, res), null);
                },
                (o, e) =>
                {
                    Logger.Log(LogSeverity.Error, this.GetType().FullName, e.Exception);
                    _syncCtxt.Post(unused => completed(e.Exception, null), null);
                });

            }
            else
            {
                var ex = new Exception(Resource.TypeDocument_SharepointContextError);
                Logger.Log(LogSeverity.Warning, this.GetType().FullName, ex);
                completed(ex, null);
            }
        }
        #endregion
    }
}
