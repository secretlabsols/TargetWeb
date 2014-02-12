<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ext="urn:ext-util">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Debtors Interface - Apply Xslt Output.
		
		Formats the Exports.Debtors.ApplyOT09Xslt output information for display.
	-->
	
	<xsl:template match="Results">
        <xsl:choose>
            <xsl:when test="string-length(OutputFilePath) > 0">
                File output to: <xsl:value-of select="OutputFilePath" /><br />
                No. of Invoices in file: <xsl:value-of select="InvCount" /><br />
                Total Amount in Invoice Headers: <xsl:value-of select="ext:FormatCurrency(AmountInHeader)" /><br />
                Total Amount in Charge Lines: <xsl:value-of select="ext:FormatCurrency(AmountOnDetails)" /><br />
                <xsl:if test="number(AmountInHeader) != number(AmountOnDetails)">
                    <b>WARNING: This Invoice File is not in balance.</b>
                    <br />
                </xsl:if>                
            </xsl:when>
            <xsl:otherwise>
                File output to: (file generation skipped - no invoices found)<br />
            </xsl:otherwise>
        </xsl:choose>
	</xsl:template>
    
</xsl:stylesheet>
