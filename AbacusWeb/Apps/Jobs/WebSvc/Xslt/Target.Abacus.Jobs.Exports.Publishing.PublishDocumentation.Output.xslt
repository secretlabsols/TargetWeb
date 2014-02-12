<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ext="urn:ext-util">
    <xsl:output method="html" indent="no" encoding="ISO-8859-1" />
    <!--
		Abacus for Windows Job Service - Publishing Interface
		
		Formats the Publishing.PublishDocumentation output information for display.
	-->
	<xsl:template match="Results">
    <xsl:if test="string-length(TargetFolder) &gt; 0">
      Destination Folder: <b><xsl:value-of select="TargetFolder" /></b><br />
      No of Master Files Published: <b><xsl:value-of select="MasterFilesCount" /></b><br />
      <xsl:if test="count(MasterFileName) &gt; 0">
        <br />
        <xsl:for-each select="MasterFileName">
          Master File Published: <b><xsl:value-of select="." /></b><br />
        </xsl:for-each>
      </xsl:if>
    </xsl:if>
  </xsl:template>
	
</xsl:stylesheet>
