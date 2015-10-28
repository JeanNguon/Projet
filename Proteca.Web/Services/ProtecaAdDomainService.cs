
namespace Proteca.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using System.DirectoryServices;
    using Proteca.Web.Models;
    using System.Collections.ObjectModel;
    using System.Configuration;

    [EnableClientAccess()]
    public class ProtecaADDomainService : DomainService
    {
        #region Properties

        private LogService _log;

        public DirectoryEntry Ldap { get; set; }

        #endregion

        #region Constructor

        public ProtecaADDomainService()
        {
            AD_Connect();
            _log = new LogService();
        }
        #endregion

        #region PublicMethods

        public void AD_Connect()
        {
            try
            {
                if (String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ActiveDirectoryUSER"].ToString()))
                {
                    Ldap = new DirectoryEntry(
                    ConfigurationManager.AppSettings["ActiveDirectoryLDAP"].ToString());
                    //Ldap.AuthenticationType = AuthenticationTypes.Delegation;
                }
                else 
                {
                    Ldap = new DirectoryEntry(
                        ConfigurationManager.AppSettings["ActiveDirectoryLDAP"].ToString(),
                        ConfigurationManager.AppSettings["ActiveDirectoryUSER"].ToString(),
                        ConfigurationManager.AppSettings["ActiveDirectoryPASSWORD"].ToString());
                }
            }
            catch (Exception ex)
            {
                _log.Log(LogLevel.Error, ex.Source, ex.Message);
            }
        }

        public List<ADUser> GetUsers()
        {
            List<ADUser> userList = null;

            try
            {
                userList = new List<ADUser>();
                DirectorySearcher searcher = new DirectorySearcher(Ldap);
                searcher.Filter = "(objectClass=user)";

                foreach (SearchResult result in searcher.FindAll())
                {
                    // On récupère l'entrée trouvée lors de la recherche
                    DirectoryEntry DirEntry = result.GetDirectoryEntry();

                    //On peut maintenant afficher les informations désirées
                    //SAMAccountName.

                    if (DirEntry.Properties["saMAccountName"].Value == null
                        || DirEntry.Properties["givenName"].Value == null
                        || DirEntry.Properties["sn"].Value == null
                        || DirEntry.Properties["mail"].Value == null)
                    {
                        userList.Add(new ADUser()
                            {
                                IsError = true
                            }
                        );
                    }
                    else
                    {
                        userList.Add(new ADUser()
                            {
                                SAMAccountName = (DirEntry.Properties["saMAccountName"].Value ?? Resources.ADResource.EmptyException.ToString()).ToString(),
                                Givenname = (DirEntry.Properties["givenName"].Value ?? Resources.ADResource.EmptyException.ToString()).ToString(),
                                SN = (DirEntry.Properties["sn"].Value ?? Resources.ADResource.EmptyException.ToString()).ToString(),
                                Email = (DirEntry.Properties["mail"].Value ?? Resources.ADResource.EmptyException.ToString()).ToString()
                            }
                        );
                    };
                }
            }
            catch (Exception ex)
            {
                _log.Log(LogLevel.Error, ex.Source, ex.Message);
                userList.Add(new ADUser()
                    {
                        IsError = true
                    }
                );
            }

            return userList;
        }

        public ADUser GetUsersByAccountName(string AccountName)
        {
            ADUser user = null;
            try
            {
                DirectorySearcher searcher = new DirectorySearcher(Ldap);
                searcher.Filter = "(saMAccountName=" + AccountName + ")";
                searcher.SearchScope = SearchScope.Subtree;

                SearchResult result = searcher.FindOne();

                // On récupère l'entrée trouvée lors de la recherche
                DirectoryEntry DirEntry = result.GetDirectoryEntry();

                if (DirEntry.Properties["saMAccountName"].Value == null
                        || DirEntry.Properties["givenName"].Value == null
                        || DirEntry.Properties["sn"].Value == null
                        || DirEntry.Properties["mail"].Value == null)
                {
                    user = new ADUser()
                    {
                        IsError = true
                    };
                }
                else
                {
                    user = new ADUser()
                    {
                        SAMAccountName = (DirEntry.Properties["saMAccountName"].Value ?? Resources.ADResource.EmptyException.ToString()).ToString(),
                        Givenname = (DirEntry.Properties["givenName"].Value ?? Resources.ADResource.EmptyException.ToString()).ToString(),
                        SN = (DirEntry.Properties["sn"].Value ?? Resources.ADResource.EmptyException.ToString()).ToString(),
                        Email = (DirEntry.Properties["mail"].Value ?? Resources.ADResource.EmptyException.ToString()).ToString()
                    };
                };
            }
            catch (Exception ex)
            {
                _log.Log(LogLevel.Error, ex.Source, ex.Message);
                user = new ADUser()
                {
                    IsError = true
                };
            }

            return user;
        }

        #endregion
    }
}


