<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ext="urn:ext-util">
    <xsl:output method="html" indent="no" encoding="ISO-8859-1" />
    <!--
		Abacus for Windows Job Service - Service Commitments Interface
		
		Formats the Commitments.ProduceOutputFile output information for display.
	-->
	<xsl:template match="Results">
        <xsl:choose>
            <xsl:when test="string-length(Filename) > 0">
                Commitment File: <xsl:value-of select="Filename" /><br />
                Total Commitment: <xsl:value-of select="ext:FormatCurrency(TotalCommitment)" /><br />
            </xsl:when>
            <xsl:otherwise>
                (No Commitment File was produced)<br />
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
	
</xsl:stylesheet>
