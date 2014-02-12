<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ext="urn:ext-util">
    <xsl:output method="html" indent="no" encoding="ISO-8859-1" />
    <!--
		Abacus for Windows Job Service - Debtors Interface (NIT only)
		
		Formats the Benefit Collection transaction output information for display.
	-->
	<xsl:template match="Results">
        <xsl:choose>
            <xsl:when test="string-length(Filename) > 0">
                File output to: <xsl:value-of select="Filename" /><br />
                No. of Benefit Transactions in file: <xsl:value-of select="TotalBenefitCount" /><br />
                Total Net Benefit: <xsl:value-of select="ext:FormatCurrency(TotalBenefitAmount)" /><br />
            </xsl:when>
            <xsl:otherwise>
                File output to: (file generation skipped - no benefit transactions found)<br />
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
	
</xsl:stylesheet>
