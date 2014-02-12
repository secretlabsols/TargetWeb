<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Service Activity Import - Convert Interface File Input.
		
		Formats the Imports.ServiceActivity.ConvertInterfaceFile input information for display.
	-->
	<xsl:template match="File">
    <xsl:choose>
      <xsl:when test="string-length(.) = 0">
        (Import file not specified)
      </xsl:when>
      <xsl:otherwise>
        The service activity file specified was: <xsl:value-of select="." /><br />
      </xsl:otherwise>
    </xsl:choose>
	</xsl:template>
	
</xsl:stylesheet>
