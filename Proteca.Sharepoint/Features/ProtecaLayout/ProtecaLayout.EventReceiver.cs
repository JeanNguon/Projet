using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Publishing;
using System.Threading;

namespace Proteca.Sharepoint.Features.ProtecaLayout
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("eb5cfc27-0677-42be-b2bc-ca3e48bab51b")]
    public class ProtecaLayoutEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite site = (SPSite)properties.Feature.Parent;
            if (PublishingSite.IsPublishingSite(site))
            {
                PublishingSite pubSite = new PublishingSite(site);


                Uri layoutURI = new Uri(site.RootWeb.Url + "_catalogs/masterpage/AccueilLayout.aspx");
                PageLayout layout = pubSite.PageLayouts[layoutURI.ToString()];
                
                updateLayout(layout);

                layoutURI = new Uri(site.RootWeb.Url + "_catalogs/masterpage/XAPLayout.aspx");
                layout = pubSite.PageLayouts[layoutURI.ToString()];

                updateLayout(layout);

                layoutURI = new Uri(site.RootWeb.Url + "_catalogs/masterpage/SearchLayout.aspx");
                layout = pubSite.PageLayouts[layoutURI.ToString()];

                updateLayout(layout);
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

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
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
