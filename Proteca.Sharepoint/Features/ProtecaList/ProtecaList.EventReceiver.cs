using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Proteca.Sharepoint.Utilities;

namespace Proteca.Sharepoint.Features.ProtecaList
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("27e50394-0c99-4743-af53-a88ff6372662")]
    public class ProtecaListEventReceiver : SPFeatureReceiver
    {
        private const string OnlineHelpGroupContentTypeId = "0x0120009FD4F98820AE594FB5D75A1B6328714E";
        private const string OnlineHelpItemContentTypeId = "0x010043c91c8eac5a4a978b57be8c9710df81";
        private const string DocuLibContentTypeId = "0x01010059642109af4546859afb2c1a23f641d0";
        private const string ModuleLibContentTypeId = "0x01010039014819D9D8F142856824CBD5FCBAAF";

        private const string LinkListContentTypeId = "0x0105";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            // Liste GED
            SPSite site = properties.Feature.Parent as SPSite;
            SPWeb web = site.RootWeb;
            SPContentType contentType = web.ContentTypes[new SPContentTypeId(DocuLibContentTypeId)];
            
            string listName = "GED";
            SPList list = ListHelper.TryCreateList(
                web,
                listName,
                SPListTemplateType.DocumentLibrary,
                new SPContentType[] { contentType },
                "Bibliothèque de document Proteca"
            );
            //// Remove the superfluous Document content types inherited by default.
            ListHelper.RemoveInheritedContentTypeFromList(web, list, SPBuiltInContentTypeId.Item);
            ListHelper.RemoveInheritedContentTypeFromList(web, list, SPBuiltInContentTypeId.Document);
            list.Update();
            
            SPContentType moduleContentType = web.ContentTypes[new SPContentTypeId(ModuleLibContentTypeId)];

            listName = "Modules";
            list = ListHelper.TryCreateList(
                web,
                listName,
                SPListTemplateType.DocumentLibrary,
                new SPContentType[] { moduleContentType },
                "Bibliothèque des Modules déportés"
            );
            //// Remove the superfluous Document content types inherited by default.
            ListHelper.RemoveInheritedContentTypeFromList(web, list, SPBuiltInContentTypeId.Item);
            ListHelper.RemoveInheritedContentTypeFromList(web, list, SPBuiltInContentTypeId.Document);
            list.Update();

            // Listes d'aide
            SPContentType groupContentType = web.ContentTypes[new SPContentTypeId(OnlineHelpGroupContentTypeId)];
            SPContentType itemContentType = web.ContentTypes[new SPContentTypeId(OnlineHelpItemContentTypeId)];
                        
            listName = "OnlineHelp";
            list = ListHelper.TryCreateList(
                web,
                listName,
                SPListTemplateType.GenericList,
                new SPContentType[] { itemContentType },
                "Liste qui fournit le contenu pour l'aide en ligne"
            );
            ListHelper.RemoveInheritedContentTypeFromList(web, list, SPBuiltInContentTypeId.Item);
            ListHelper.RemoveInheritedContentTypeFromList(web, list, SPBuiltInContentTypeId.Folder);
            list.Update();

            listName = "Glossary";
            list = ListHelper.TryCreateList(
                web,
                listName,
                SPListTemplateType.GenericList,
                new SPContentType[] { itemContentType },
                "Liste qui fournit le contenu pour le glossaire"
            );
            ListHelper.RemoveInheritedContentTypeFromList(web, list, SPBuiltInContentTypeId.Item);
            ListHelper.RemoveInheritedContentTypeFromList(web, list, SPBuiltInContentTypeId.Folder);
            list.Update();

            listName = "DiagnosticHelp";
            list = ListHelper.TryCreateList(
                web,
                listName,
                SPListTemplateType.GenericList,
                new SPContentType[] { groupContentType, itemContentType },
                "Liste qui fournit le contenu pour l'aide diagnostique"
            );
            ListHelper.RemoveInheritedContentTypeFromList(web, list, SPBuiltInContentTypeId.Item);
            ListHelper.RemoveInheritedContentTypeFromList(web, list, SPBuiltInContentTypeId.Folder);
            list.Update();


            // Listes de lien pour la page d'accueil
            //SPContentType linkListContentType = web.ContentTypes[new SPContentTypeId(LinkListContentTypeId)];

            listName = "HomeLink";
            list = ListHelper.TryCreateList(
                web,
                listName,
                SPListTemplateType.Links,
                new SPContentType[] { }, //new SPContentType[] { linkListContentType },
                "Liste qui fournit les lien de la page d'accueil"
            );
            list.Update();
        }
    }
}
