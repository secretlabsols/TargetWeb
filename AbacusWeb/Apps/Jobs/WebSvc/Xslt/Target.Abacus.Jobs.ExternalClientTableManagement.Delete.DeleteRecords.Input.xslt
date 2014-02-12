<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Remove Historic Entries from External Client Table
		
		Formats the ExternalClientTableManagement.Delete.DeleteRecords input information for display.
	-->
    <xsl:template match="Inputs">
        <xsl:choose>
            <xsl:when test="string-length(FilterDeleteBeforeDate) &gt; 0">
                Input: <xsl:value-of select="FilterDeleteBeforeDate" /><br />
            </xsl:when>
            <xsl:otherwise>
                Input: (No Input Found)<br />
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
</xsl:stylesheet>
