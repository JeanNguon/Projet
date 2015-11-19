using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.Security;
using System.Security.Permissions;
using System.Text;
using System.Collections.Generic;
using Microsoft.SharePoint.Utilities;

namespace Proteca.Sharepoint.SilverlightWebPart
{
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
    [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
    public class SilverlightWebPart : ClientApplicationWebPartBase
    {
        /// <summary>
        /// Edition du InitParams du composant Silverlight
        /// </summary>
        [WebBrowsable(true),
	    Category("Configuration"),
	    Personalizable(PersonalizationScope.Shared),
	    WebDisplayName("Paramètres d'initialisation"),
        WebDescription("Paramètres d'initialisation")]
	    public string InitParams { get; set; }

        /// <summary>
        /// Chemin vers le fichier XAP
        /// </summary>
        [WebBrowsable(true),
        Category("Configuration"),
        Personalizable(PersonalizationScope.Shared),
        WebDisplayName("Chemin vers le fichier XAP"),
        WebDescription("Chemin vers le fichier XAP")]
        public string XAPFilePath { get; set; }

        /// <summary>
        /// Hauteur du XAP
        /// </summary>
        [WebBrowsable(true),
        Category("Configuration"),
        Personalizable(PersonalizationScope.Shared),
        WebDisplayName("Hauteur du XAP"),
        WebDescription("Hauteur du XAP")]
        public string SLHeight { get; set; }

        /// <summary>
        /// Largeur du XAP
        /// </summary>
        [WebBrowsable(true),
        Category("Configuration"),
        Personalizable(PersonalizationScope.Shared),
        WebDisplayName("Largeur du XAP"),
        WebDescription("Largeur du XAP")]
        public string SLWidth { get; set; }

        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Proteca.Sharepoint/SilverlightWebPart/SilverlightWebPartUserControl.ascx";

        /// <summary>
        /// Constructeur
        /// </summary>
        protected override void CreateChildControls()
        {
            SilverlightWebPartUserControl control = (SilverlightWebPartUserControl)Page.LoadControl(_ascxPath);
            control.XAPFilePath = XAPFilePath;
            control.InitParams = GetInitParameters();
            control.SLHeight = SLHeight;
            control.SLWidth = SLWidth;
            Controls.Add(control);
        }

        /// <summary>
        /// Get the Sharepoint Init Params and the custom init Params.
        /// </summary>
        /// <returns></returns>
        private string GetInitParameters()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string initParams = base.GetInitParams();
            stringBuilder.Append(initParams);
            if (!string.IsNullOrEmpty(this.InitParams))
            {
                Dictionary<string, string> strs = this.ProcessCustomInitParameters(this.InitParams);
                foreach (string key in strs.Keys)
                {
                    stringBuilder.Append(",");
                    stringBuilder.Append(SPHttpUtility.UrlKeyValueEncode(key));
                    stringBuilder.Append("=");
                    stringBuilder.Append(SPHttpUtility.UrlKeyValueEncode(strs[key]));
                }
            }
            return stringBuilder.ToString();
        }


        /// <summary>
        /// Check Custom Parameters
        /// </summary>
        /// <param name="strCustomInitParameters"></param>
        /// <returns></returns>
        private Dictionary<string, string> ProcessCustomInitParameters(string strCustomInitParameters)
        {
            string str;
            string serverRelativeUrl;
            string empty;
            Dictionary<string, string> strs = new Dictionary<string, string>();
            char[] chrArray = new char[1];
            chrArray[0] = ',';
            string[] strArrays = strCustomInitParameters.Split(chrArray);
            string[] strArrays1 = strArrays;
            for (int i = 0; i < (int)strArrays1.Length; i++)
            {
                string str1 = strArrays1[i];
                int num = str1.IndexOf('=');
                if (num != -1)
                {
                    str = str1.Substring(0, num).Trim();
                    if (num < str1.Length - 1)
                    {
                        empty = str1.Substring(num + 1).Trim();
                    }
                    else
                    {
                        empty = string.Empty;
                    }
                    serverRelativeUrl = empty;
                    if (string.Compare(serverRelativeUrl, "{webpartid}", StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        if (string.Compare(serverRelativeUrl, "{pageurl}", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            serverRelativeUrl = ((SPWebPartManager)base.WebPartManager).ServerRelativeUrl;
                            if (serverRelativeUrl == null)
                            {
                                serverRelativeUrl = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        Guid storageKey = ((SPWebPartManager)base.WebPartManager).GetStorageKey(this);
                        serverRelativeUrl = storageKey.ToString("B").ToUpperInvariant();
                    }
                }
                else
                {
                    str = str1.Trim();
                    serverRelativeUrl = string.Empty;
                }
                if (str.Length != 0 && string.Compare(str, "MS.SP.url", StringComparison.Ordinal) != 0 && string.Compare(str, "MS.SP.formDigest", StringComparison.Ordinal) != 0 && string.Compare(str, "MS.SP.formDigestTimeoutSeconds", StringComparison.Ordinal) != 0 && string.Compare(str, "MS.SP.requestToken", StringComparison.Ordinal) != 0 && string.Compare(str, "MS.SP.viaUrl", StringComparison.Ordinal) != 0 && !strs.ContainsKey(str))
                {
                    strs.Add(str, serverRelativeUrl);
                }
            }
            return strs;
        }
    }
}
