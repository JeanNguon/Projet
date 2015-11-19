using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using System.IO;

namespace Proteca.Sharepoint
{
    public class ClientAccessPolicyDeploymentJob : SPJobDefinition
    {
        public ClientAccessPolicyDeploymentJob()
            :base()
        {
                
        }

        public ClientAccessPolicyDeploymentJob(string jobName, SPService service, SPServer server, SPJobLockType targetType)
            :base(jobName, service, server, targetType)
        {
        }

        public ClientAccessPolicyDeploymentJob(string jobName, SPWebApplication webApplication)
            : base(jobName, webApplication, null, SPJobLockType.None)
        {
        }

        public override void Execute(Guid targetInstanceId)
        {
            var webApp = this.Parent as SPWebApplication;
            foreach (KeyValuePair<SPUrlZone, SPIisSettings> setting in webApp.IisSettings)
            {
                var webRootPolicyLocation = setting.Value.Path.FullName + @"\clientaccesspolicy.xml";
                var featuresPolicyLocation = SPUtility.GetGenericSetupPath(@"TEMPLATE\FEATURES\Proteca.Sharepoint_DeployClientAccessPolicy\ClientAccessPolicy\clientaccesspolicy.xml");
                File.Copy(featuresPolicyLocation, webRootPolicyLocation, true);
            }

            base.Execute(targetInstanceId);
        }

    }
}
