using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.Office.SecureStoreService.Server;
using Microsoft.BusinessData.Infrastructure.SecureStore;

namespace Proteca.Sharepoint.Utilities
{
    // Les propriétés de connections sont hébergé dans un secure store

    public class SecurityStoreHelper
    {
        const string DB_APP_ID = "BCSProteca";

        public static string GetDBConnectionString()
        {
            string dataSource = null, userName = null, password = null;
            string result = null;
            ISecureStoreProvider secureStoreProvider = SecureStoreProviderFactory.Create();

            using (SecureStoreCredentialCollection credentialCollection = secureStoreProvider.GetCredentials(DB_APP_ID))
            {
                foreach (SecureStoreCredential credential in credentialCollection)
                {
                     switch (credential.CredentialType)
                    {
                        case SecureStoreCredentialType.Generic:
                            dataSource = GetStringFromSecureString(credential.Credential);
                            break;
                        case SecureStoreCredentialType.UserName:
                            userName = GetStringFromSecureString(credential.Credential);
                            break;
                        case SecureStoreCredentialType.Password:
                            password = GetStringFromSecureString(credential.Credential);
                            break;
                    }
                }
            }
            result = dataSource + "User ID=" + userName + ";Password=" + password;
            return result;
        }
        
        // Conversion des string du secure store en string lisible
        private static string GetStringFromSecureString(SecureString secStr)
        {
            if (secStr == null)
            {
                return null;
            }

            IntPtr pPlainText = IntPtr.Zero;
            try
            {
                pPlainText = Marshal.SecureStringToBSTR(secStr);
                return Marshal.PtrToStringBSTR(pPlainText);
            }
            finally
            {
                if (pPlainText != IntPtr.Zero)
                {
                    Marshal.FreeBSTR(pPlainText);
                }
            }
        }
    }
}
