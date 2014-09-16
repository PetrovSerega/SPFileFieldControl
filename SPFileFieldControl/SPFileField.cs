using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

namespace SPFileFieldControl
{
    public class SPFileField : SPField 
    {
        // Реализуем необходимые для SPField конструкторы
        public SPFileField(SPFieldCollection fields, string fieldName) : base(fields, fieldName) { }

        public SPFileField(SPFieldCollection fields, string typeName, string displayName) : base(fields, typeName, displayName) { }

        public override BaseFieldControl FieldRenderingControl
        {
            get
            {
                // Получаем задаваемые пользователем свойства
                string libraryName = Convert.ToString(this.GetCustomProperty("LibraryName"));

                // Создаём и возвращаем контрол для работы с полем
                BaseFieldControl ctrl = new SPFileFieldControl(libraryName);
                ctrl.FieldName = this.InternalName;
                return ctrl;
            }
        }

        // По строке получаем значение FileValue
        public override object GetFieldValue(string value)
        {
            if (String.IsNullOrEmpty(value))
                return null;

            return new FileValue(value);
        }
    }

    public class SPFileFieldControl : BaseFieldControl 
    {
        // Сообщение, которое выводим в случае отсутствия файла
        private const string fileNotExist = "Файл не загружен";

        // Имя шаблона для формы редактирования
        private const string editTemplateName = "SPFileFieldControlEdit";

        // Имя шаблона для формы отображения
        private const string displayTemplateName = "SPFileFieldControlDisplay";

        // Имя библиотеки, где храним файлы
        public string libraryName;

        // Переопределяем свойство Value, так чтобы работать с FileValue
        private FileValue currentFile = null;
        public override object Value
        {
            get
            {
                return currentFile;
            }
            set
            {
                currentFile = (FileValue)value;
            }
        }

        public SPFileFieldControl(string aLibraryName)
        {
            libraryName = aLibraryName;
        }

        // Получаем значение текущего файла
        protected override void OnInit(EventArgs e)
        {
            currentFile = (FileValue)this.ItemFieldValue;
            base.OnInit(e);
        }

        // Подключаем шаблоны, которые будут использоваться для отрисовки
        protected override string DefaultTemplateName
        {
            get
            {
                return base.ControlMode == SPControlMode.Display
                           ? displayTemplateName
                           : editTemplateName;
            }
        }

        public override string DisplayTemplateName
        {
            get { return displayTemplateName; }
        }

        // После создания контролов по заданному шаблону задаём некоторые их свойства
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            if (base.ControlMode == SPControlMode.Display)
            {
                SetupDisplayTemplateControls();
            }
            else
            {
                SetupEditTemplateControls();
            }
        }

        // Настраиваем контролы для отображения поля на форме редактирования
        private void SetupEditTemplateControls()
        {
            FileUpload fuDocument = (FileUpload)TemplateContainer.FindControl("fuDocument");
            HyperLink aFile = (HyperLink)TemplateContainer.FindControl("aFile");
            HiddenField hdFileName = (HiddenField)TemplateContainer.FindControl("hdFileName");
            HtmlInputImage btnDelete = (HtmlInputImage)TemplateContainer.FindControl("btnDelete");
            HtmlInputImage btnAdd = (HtmlInputImage)TemplateContainer.FindControl("btnAdd");

            // В зависимости от того загружен ли файл отображаем контролы
            if (currentFile != null)
            {
                // Получаем ссылку для загрузки файла
                SPWeb web = SPContext.Current.Site.RootWeb;
                SPList list = web.GetList(SPUrlUtility.CombineUrl(web.ServerRelativeUrl, libraryName));
                SPListItem spFileItem = list.GetItemByUniqueId(currentFile.UniqueID);

                // Показываем ссылку на файл
                aFile.NavigateUrl = SPUrlUtility.CombineUrl(web.ServerRelativeUrl, spFileItem.Url);
                aFile.Text = currentFile.Name;

                // А в скрытом поле сохраняем UniqueId файла
                hdFileName.Value = spFileItem.UniqueId.ToString();
            }
            else
            {
                // Отображаем сообщение о том, что файл не задан
                aFile.Text = fileNotExist;
                hdFileName.Value = String.Empty;
            }

            btnDelete.Attributes.Add("onclick", String.Format(@"clearFileValue('{0}','{1}','{2}','{3}');return false;",
                                                              aFile.ClientID, fuDocument.ClientID, hdFileName.ClientID,
                                                              fileNotExist));
            btnAdd.Attributes.Add("onclick", String.Format(@"changeDisplay('{0}');return false;", fuDocument.ClientID));
            fuDocument.Attributes.Add("onchange",String.Format(@"changeFileName(this,'{0}');return false;", aFile.ClientID));
        }

        // Настраиваем контролы для отображения поля на форме отображения
        private void SetupDisplayTemplateControls()
        {
            if (currentFile != null)
            {
                // Получаем ссылку для загрузки файла
                SPWeb web = SPContext.Current.Site.RootWeb;
                SPList list = web.GetList(SPUrlUtility.CombineUrl(web.ServerRelativeUrl, libraryName));
                SPListItem spFileItem = list.GetItemByUniqueId(currentFile.UniqueID);

                // Показываем ссылку на файл
                HyperLink aFile = (HyperLink)TemplateContainer.FindControl("aFile");
                aFile.NavigateUrl = SPUrlUtility.CombineUrl(web.ServerRelativeUrl, spFileItem.Url);
                aFile.Text = currentFile.Name;
            }
        }

        // Обновляем поле после изменений пользователя
        public override void UpdateFieldValueInItem()
        {
            // Проверяем валидность страницы
            Page.Validate();
            if (Page.IsValid)
            {
                // Посмотрим, что пришло с клиента
                FileUpload fuDocument = (FileUpload)TemplateContainer.FindControl("fuDocument");
                HiddenField hdFileName = (HiddenField)TemplateContainer.FindControl("hdFileName");

                // Если скрытое поле пусто и есть файл, значит, его нужно удалить
                if (hdFileName.Value == String.Empty && currentFile != null)
                {
                    // Удаляем файл
                    deleteFile();

                    // Обнуляем значение поля
                    currentFile = null;
                }

                // Загружаем файл и выставляем на него ссылки
                if (fuDocument.HasFile)
                {
                    SPWeb web = SPContext.Current.Site.RootWeb;
                    SPFolder folder = web.GetFolder(SPUrlUtility.CombineUrl(web.ServerRelativeUrl, libraryName));

                    // Если файл был, удалим его
                    if (currentFile != null)
                    {
                        deleteFile();
                    }

                    // Добавим новый файл и сохраним соответствующие значения
                    currentFile = addFile(folder, fuDocument);
                }

                base.UpdateFieldValueInItem();
            }
        }

        // Удаляем файл
        private void deleteFile()
        {
            // Получаем элемент из библиотеки документов по UniqueID и удаляем его
            SPWeb web = SPContext.Current.Site.RootWeb;
            SPList list = web.GetList(SPUrlUtility.CombineUrl(web.ServerRelativeUrl, libraryName));
            SPListItem spFileItem = list.GetItemByUniqueId(currentFile.UniqueID);
            spFileItem.Delete();
        }

        // Добавляем файл
        private FileValue addFile(SPFolder folder, FileUpload fuDocument)
        {
            string uniqueName = Guid.NewGuid().ToString();
            // Добавляем файл с именем uniqueName
            SPFile spfile = folder.Files.Add(uniqueName, fuDocument.FileContent,true);

            // Обновляем имя нового элемента библиотеки документов
            spfile.Item["BaseName"] = String.Format("{1}-{0}", fuDocument.FileName, spfile.Item.ID);
            spfile.Item.Update();

            return new FileValue()
            {
                Url = spfile.Item.Url,
                UniqueID = spfile.Item.UniqueId,
                Name = fuDocument.FileName
            };
        }
    }

    public class FileValue : SPFieldMultiColumnValue
    {
        private const int c_PropNumber = 3;

        public FileValue() : base(c_PropNumber) {}

        public FileValue(string value) : base(value) {}

        public string Name
        {
            get { return base[0]; }
            set { base[0] = value; }
        }

        public string Url
        {
            get { return base[1]; }
            set { base[1] = value; }
        }

        public Guid _UniqueID = Guid.Empty;
        public Guid UniqueID
        {
            get
            {
                if (_UniqueID == Guid.Empty)
                    _UniqueID = new Guid(base[2]);
                return _UniqueID;
            }
            set
            {
                _UniqueID = value;
                base[2] = value.ToString();
            }
        }
    }
}
