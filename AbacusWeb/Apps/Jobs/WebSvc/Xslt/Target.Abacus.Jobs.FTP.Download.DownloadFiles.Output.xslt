<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
	    Abacus for Windows Job Service - Download Files from FTP Server
		
		Formats the FTP.Download.DownLoadFiles output information for display.
	-->
    <xsl:template match="Results">
        No. of files uploaded: <xsl:value-of select="FileCount" /><br />
        <br /><br />
        <xsl:if test="string-length(actionsCSVFileURL) &gt; 0">
            Please see the actions report for more details:
            <br />
            View the report <a href="{actionsCSVFileURL}">in Excel</a>
            <br />
            View the report <a href="{actionsXMLFileURL}">as XML</a>
        </xsl:if>
    </xsl:template>
</xsl:stylesheet>