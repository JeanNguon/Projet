using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System.Collections;
using Microsoft.SharePoint.Publishing;

namespace Proteca.Sharepoint.Features.ProtecaMasterPage
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("22e71a91-ee9c-4d67-b5e9-f14bf359208e")]
    public class ProtecaMasterPageEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite currentSite = (SPSite)properties.Feature.Parent;
            if (PublishingSite.IsPublishingSite(currentSite))
            {
                PublishingSite pubSite = new PublishingSite(currentSite);
                
                SPWeb currentWeb = currentSite.RootWeb;

                SPFile myStyle = currentWeb.GetFile("Style Library/proteca.css");
                if (myStyle != null && myStyle.Exists)
                {
                    if (myStyle.CheckedOutByUser != null)
                    {
                        myStyle.UndoCheckOut();
                        myStyle.Update();
                    }
                    myStyle.CheckOut();
                    myStyle.Update();
                    myStyle.CheckIn("Automatically checked in by PROTECAMasterPage feature", SPCheckinType.MajorCheckIn);
                    myStyle.Update();
                }

                Uri masterURI = new Uri(currentWeb.Url + "/_catalogs/masterpage/PROTECA.master");

                SPFile masterFile = currentWeb.GetFile("/_catalogs/masterpage/PROTECA.master");
                if (masterFile != null && masterFile.Exists)
                {
                    if (masterFile.CheckedOutByUser != null)
                    {
                        masterFile.UndoCheckOut();
                        masterFile.Update();
                    }
                    masterFile.CheckOut();
                    masterFile.Update();
                    masterFile.CheckIn("Automatically checked in by PROTECAMasterPage feature", SPCheckinType.MajorCheckIn);
                    masterFile.Update();
                }


                currentWeb.AllowUnsafeUpdates = true;

                //currentWeb.MasterUrl = masterURI.AbsolutePath;

                currentWeb.CustomMasterUrl = masterURI.AbsolutePath;

                currentWeb.Update();
                currentWeb.AllowUnsafeUpdates = false;

                foreach (SPWeb subWeb in currentSite.AllWebs)
                {
                    if (subWeb.IsRootWeb) continue;

                    subWeb.AllowUnsafeUpdates = true;
                    Hashtable hash = subWeb.AllProperties;
                    subWeb.MasterUrl = subWeb.ParentWeb.MasterUrl;
                    hash["__InheritsMasterUrl"] = "True";

                    subWeb.CustomMasterUrl = subWeb.ParentWeb.CustomMasterUrl;
                    hash["__InheritsCustomMasterUrl"] = "True";

                    subWeb.Update();
                    subWeb.AllowUnsafeUpdates = false;
                }
            }
        }

        /// <summary>
        /// Update Layout
        /// </summary>
        /// <param name="layout"></param>
        private void updateLayout(PageLayout layout)
        {
            SPFile file = layout.ListItem.File;
            if (!layout.ListItem.HasPublishedVersion)
            {
                file.CheckIn("Automatically checked in by PROTECALayout feature", SPCheckinType.MajorCheckIn);
                file.Update();

                file.Approve("Automatically approved by PROTECALayout feature");
                file.Update();
            }
            else
            {
                if (file.CheckedOutByUser != null)
                {
                    file.UndoCheckOut();
                    file.Update();
                }

                file.CheckOut();
                file.Update();
                file.CheckIn("Automatically checked in by PROTECALayout feature", SPCheckinType.MajorCheckIn);
                file.Update();
                file.Approve("Automatically approved by PROTECALayout feature");
                file.Update();
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPSite currentSite = properties.Feature.Parent as SPSite;
            SPWeb currentWeb = currentSite.RootWeb;

            Uri masterURI = new Uri(currentWeb.Url + "/_catalogs/masterpage/v4.master");

            //currentWeb.MasterUrl = masterURI.AbsolutePath;

            currentWeb.CustomMasterUrl = masterURI.AbsolutePath;

            currentWeb.Update();

            foreach (SPWeb subWeb in currentSite.AllWebs)
            {
                if (subWeb.IsRootWeb) continue;
                Hashtable hash = subWeb.AllProperties;
                subWeb.MasterUrl = subWeb.ParentWeb.MasterUrl;
                hash["__InheritsMasterUrl"] = "True";

                subWeb.CustomMasterUrl = subWeb.ParentWeb.CustomMasterUrl;
                hash["__InheritsCustomMasterUrl"] = "True";

                subWeb.Update();
            }
        }


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
