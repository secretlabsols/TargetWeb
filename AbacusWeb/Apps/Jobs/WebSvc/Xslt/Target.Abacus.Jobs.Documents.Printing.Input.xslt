<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Documents Printing
		
		Formats the input information for display.
	-->
    <xsl:template match="Inputs">

		<b>Created: </b><xsl:value-of select="CreatedDate" /> by <xsl:value-of select="CreatedBy" /><br />

        <b>Document Count: </b><xsl:value-of select="DocumentCount" /> <br />

        <b>Printer: </b><xsl:value-of select="PrinterName" /> 

        <xsl:if test="string-length(Comment) &gt; 0">
            <br />
            <b>Comment: </b><xsl:value-of select="Comment" />
        </xsl:if>

    </xsl:template>
</xsl:stylesheet>
