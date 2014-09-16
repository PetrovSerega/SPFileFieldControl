<%@ Control Language="C#" %>
<%@ Assembly Name="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="System.Net.Mime" %>
<%@ Register TagPrefix="SharePoint"
Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"
Namespace="Microsoft.SharePoint.WebControls" %>
<%-- Шаблон для формы редактирования --%>
<SharePoint:RenderingTemplate ID="SPFileFieldControlEdit" runat="server">
    <Template>
        <%-- Подключаем CSS и JS --%>
        <SharePoint:CssRegistration ID="CssRegistration1" name="/_layouts/styles/FileFieldControl.css"  after="corev4.css" runat="server"/>
        <SharePoint:ScriptLink ID="ScriptLink1" runat="server" Name="FileFieldControl.js" Localizable="false"/>
        <%-- Гиперссылка для загрузки --%>
        <asp:HyperLink ID="aFile" runat="server" CssClass="ffc-hl"  EnableViewState="False"/>
        <%-- Кнопка удалить - очищает гиперссылку и скрытое поле) --%>
        <input type="image" ID="btnDelete" runat="server" src="/_layouts/images/DELETE.gif" alt="delete"/>
        <%-- Кнопка добавить - показывает FileUpload --%>
        <input type="image" ID="btnAdd" runat="server" src="/_layouts/images/newrowheader.png" alt="add"/>
        <br/>
        <asp:FileUpload ID="fuDocument" runat="server" CssClass="ffc-fu" />     
        <%-- В скрытом поле храним UniqueId чтобы в случае необходимо удалить файл --%>
        <asp:HiddenField runat="server" ID="hdFileName"/>
    </Template>
</SharePoint:RenderingTemplate>
<%-- Шаблон для формы отображения --%>
<SharePoint:RenderingTemplate ID="SPFileFieldControlDisplay" runat="server">
    <Template>
        <%-- Подключаем CSS --%>
        <SharePoint:CssRegistration ID="CssRegistration1" name="/_layouts/styles/FileFieldControl.css"  after="corev4.css" runat="server"/>
        <%-- Гиперссылка для загрузки --%>
        <asp:HyperLink ID="aFile" runat="server" CssClass="ffc-hl"  EnableViewState="False"/>
    </Template>
</SharePoint:RenderingTemplate>