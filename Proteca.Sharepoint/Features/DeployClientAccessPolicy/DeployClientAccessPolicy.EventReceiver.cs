using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Administration;

namespace Proteca.Sharepoint.Features.DeployClientAccessPolicy
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("cf46cb56-2861-4803-80fc-a8f48838f903")]
    public class DeployClientAccessPolicyEventReceiver : SPFeatureReceiver
    {
        private const string JobName = "ClientAccessPolicyJob";

        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var app = properties.Feature.Parent as SPWebApplication;
            RemoveJobIfRegistered(app);

            var clientAccessPolicyJob = new ClientAccessPolicyDeploymentJob(JobName, app);
            var schedule = new SPOneTimeSchedule(DateTime.Now);
            clientAccessPolicyJob.Schedule = schedule;
            clientAccessPolicyJob.Update();

            app.JobDefinitions.Add(clientAccessPolicyJob);
            app.Update();

            clientAccessPolicyJob.RunNow();
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var app = properties.Feature.Parent as SPWebApplication;

            RemoveJobIfRegistered(app);
        }

        private void RemoveJobIfRegistered(SPWebApplication app)
        {
            foreach (SPJobDefinition job in app.JobDefinitions)
            {
                if (job.Title == JobName)
                {
                    job.Delete();
                }
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
