using System;
using System.Reflection;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace SPFileFieldControl.ER_OnFileFieldCRUD
{
    public class ER_OnFileFieldCRUD : SPListEventReceiver
    {
        public const string SPFileFieldName = "SPFileField";

        public override void FieldAdded(SPListEventProperties properties)
        {
            // Если добавляется поле SPFileField, то подписываемся на удаление элементов, чтобы удалять связные файлы
            if (properties.Field.TypeDisplayName == SPFileFieldName)
            {
                // Пробуем добавить EventReceiver, если его нет
                bool eventReceiverExists = false;
                for (int i = 0; i < properties.List.EventReceivers.Count;i++)
                {
                    if (properties.List.EventReceivers[i].Class ==
                        ER_OnItemDeleting.ER_OnItemDeleting.ER_OnItemDeletingName)
                    {
                        eventReceiverExists = true;
                        break;
                    }
                }

                if (!eventReceiverExists)
                {
                    properties.List.EventReceivers.Add(SPEventReceiverType.ItemDeleting, 
                                                       Assembly.GetExecutingAssembly().FullName, 
                                                       ER_OnItemDeleting.ER_OnItemDeleting.ER_OnItemDeletingName);
                }
            }

            base.FieldAdded(properties);
        }

        public override void FieldDeleting(SPListEventProperties properties)
        {
            if (properties.Field.TypeDisplayName == SPFileFieldName)
            {
                // Если удаляется столбец SPFileField, то
                // 1) Удаляем все связные файлы, если они есть
                for (int i = 0; i < properties.List.Items.Count; i++)
                {
                    SPListItem splistItem = properties.List.Items[i];
                    FileValue fileValue = splistItem[properties.Field.StaticName] as FileValue;

                    if (fileValue == null)
                        continue;

                    SPFile spFile = properties.Web.GetFile(fileValue.UniqueID);
                    spFile.Delete();
                }

                // Проверяем, останутся ли после удаления столбцы типа SPFileField
                bool anyFileFieldExist = false;
                for (int i = 0; i < properties.List.Fields.Count; i++)
                {
                    if (properties.List.Fields[i].TypeAsString == SPFileFieldName
                        && properties.List.Fields[i].StaticName != properties.Field.StaticName)
                    {
                        anyFileFieldExist = true;
                        break;
                    }
                }

                // 2) Отписываемся от триггера, если больше нет колонок типа SPFileField
                if (!anyFileFieldExist)
                {
                    for (int i = 0; i < properties.List.EventReceivers.Count; )
                    {
                        if (properties.List.EventReceivers[i].Class == ER_OnItemDeleting.ER_OnItemDeleting.ER_OnItemDeletingName)
                        {
                            properties.List.EventReceivers[i].Delete();
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
            }

            base.FieldDeleting(properties);
        }
    }
}