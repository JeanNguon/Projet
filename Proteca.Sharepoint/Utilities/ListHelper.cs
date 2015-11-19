using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Proteca.Sharepoint.Utilities
{
    public static class ListHelper
    {
        public static void RemoveInheritedContentTypeFromList(SPWeb web, SPList list, SPContentTypeId contentTypeId)
        {
            SPContentType ctWeb = web.ContentTypes[contentTypeId];
            SPContentType ctList = list.ContentTypes[ctWeb.Name];
            if (ctList != null)
            {
                list.ContentTypes.Delete(ctList.Id);
            }
        }

        public static SPList TryCreateList(SPWeb web, string listName, SPListTemplateType templateType, SPContentType[] contentTypes, string listDescription)
        {
            SPList list = web.Lists.TryGetList(listName);
            if (list == null)
            {
                Guid listId = web.Lists.Add(listName, listDescription, templateType);
                list = web.Lists[listId];
                list.ContentTypesEnabled = true;
                foreach (SPContentType ct in contentTypes)
                {
                    list.ContentTypes.Add(ct);
                }
                list.OnQuickLaunch = false;
                list.Update();
            }
            return list;
        }
    }
}
