using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace SPFileFieldControl.ER_OnItemDeleting
{
    public class ER_OnItemDeleting : SPItemEventReceiver
    {
        public const string ER_OnItemDeletingName =
            "SPFileFieldControl.ER_OnItemDeleting.ER_OnItemDeleting";

        public const string SPFileFieldName = "SPFileField";

        public override void ItemDeleting(SPItemEventProperties properties)
        {
            // Проходим по всем полям удаляемого элемента списка
            for (int i = 0; i < properties.ListItem.Fields.Count; i++)
            {
                // Если поле типа SPFileField
                if (properties.ListItem.Fields[i].TypeAsString == ER_OnItemDeleting.SPFileFieldName)
                {
                    // И был загружен файл
                    FileValue fileValue = properties.ListItem[properties.ListItem.Fields[i].StaticName] as FileValue;
                    if (fileValue == null)
                        continue;

                    // Удаляем его
                    SPFile spFile = properties.Web.GetFile(fileValue.UniqueID);
                    spFile.Delete();
                }
            }

            base.ItemDeleting(properties);
        }
    }
}