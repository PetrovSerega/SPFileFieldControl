<xsl:stylesheet xmlns:x="http://www.w3.org/2001/XMLSchema"
        xmlns:d="http://schemas.microsoft.com/sharepoint/dsp"
        version="1.0"
        exclude-result-prefixes="xsl msxsl ddwrt"
        xmlns:ddwrt="http://schemas.microsoft.com/WebParts/v2/DataView/runtime"
        xmlns:asp="http://schemas.microsoft.com/ASPNET/20"
        xmlns:__designer="http://schemas.microsoft.com/WebParts/v2/DataView/designer"
        xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
        xmlns:msxsl="urn:schemas-microsoft-com:xslt"
        xmlns:SharePoint="Microsoft.SharePoint.WebControls"
        xmlns:ddwrt2="urn:frontpage:internal">
  <!-- Этот шаблон будет использован для столбцов типа SPFileField-->
  <xsl:template match="FieldRef[@FieldType='SPFileField']" mode="Text_body">
    <!-- Получаем значение поля -->
    <xsl:param name="thisNode" select="."/>
    <xsl:variable name="full" select="$thisNode/@*[name()=current()/@Name]" />
    <!-- На первой позиции имя, на второй путь к файлу (на третьей UniqueID, который здесь не используется) -->
    <!-- "Вырезаем" имя и путь к файлу -->
    <xsl:variable name="name" select="substring-before(substring-after($full,';#'),';#')" />
    <xsl:variable name="url" select="substring-before(substring-after(substring-after($full,';#'),';#'),';#')" />
    <!-- Выводим гиперссылку для загрузки файла -->
    <xsl:element name="a">
      <xsl:attribute name="href">
        <xsl:value-of select="concat($RootSiteUrl,'/',$url)"/>
      </xsl:attribute>
      <xsl:value-of select="$name"/>
    </xsl:element>
  </xsl:template>  
</xsl:stylesheet>
