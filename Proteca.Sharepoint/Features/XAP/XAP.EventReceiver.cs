using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using System.IO;

namespace Proteca.Sharepoint.Features.XAP
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("eafccb57-006d-423e-8904-053ed5a68f88")]
    public class XAPEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        //public override void FeatureActivated(SPFeatureReceiverProperties properties)
        //{
        //    var web = properties.Feature.Parent as SPWeb;

        //    var featuresPolicyLocation = SPUtility.GetGenericSetupPath(@"TEMPLATE\FEATURES\Proteca.Sharepoint_XAP\ClientAccessPolicy\clientaccesspolicy.xml");

        //    web.RootFolder.Files.Add("clientaccesspolicy.xml",File.OpenRead(featuresPolicyLocation), true);
        //}


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //    var web = properties.Feature.Parent as SPWeb;
        //    SPFile fileToDelete = null;
        //    foreach (SPFile file in web.RootFolder.Files)
        //    {
        //        if (file.Url == "clientaccesspolicy.xml")
        //        {
        //            fileToDelete = file;
        //            break;
        //        }
        //    }
        //    if (fileToDelete != null)
        //    {
        //        fileToDelete.Delete();
        //    }
        //}


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
