<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Remove Historic Entries from External Client Table
		
		Formats the ExternalClientTableManagement.Delete.DeleteRecords output information for display.
	-->
    <xsl:template match="Results">
        No. of external client detail records deleted: <xsl:value-of select="DeletionCount" /><br />
        <br /><br />
        <xsl:if test="string-length(ReportCSVFileURL) &gt; 0">
            Please see the deletion report for more details:
            <br />
            View the report <a href="{ReportCSVFileURL}">in Excel</a>
            <br />
            View the report <a href="{ReportXMLFileURL}">as XML</a>
        </xsl:if>
    </xsl:template>
</xsl:stylesheet>