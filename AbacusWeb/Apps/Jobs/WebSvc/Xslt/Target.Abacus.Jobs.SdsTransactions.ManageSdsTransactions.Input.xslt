<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Managing SDS Transactions
		
		Formats the input information for display.
	-->
    <xsl:template match="Inputs">
        <xsl:choose>
            <xsl:when test="string-length(FilterServiceUserID) &gt; 0 and number(FilterServiceUserID) &gt; 0">
                Selected Service User ID: <xsl:value-of select="FilterServiceUserReferenceAndName" /><br />
            </xsl:when>
            <xsl:otherwise>
                Selected Service User ID: (All)<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(FilterForceReconsideration) &gt; 0 and FilterForceReconsideration = 'True'">
                Force Reconsideration? Yes<br />
            </xsl:when>
            <xsl:otherwise>
                Force Reconsideration? No<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(FilterTransactionTypes) &gt; 0 and number(FilterTransactionTypes) &gt; 0">
                Selected SDS Transaction Type: <xsl:value-of select="FilterSdsTransactionTypes" /><br />
            </xsl:when>
            <xsl:otherwise>
                Selected SDS Transaction Type: (All)<br />
            </xsl:otherwise>
        </xsl:choose>        
    </xsl:template>
</xsl:stylesheet>
