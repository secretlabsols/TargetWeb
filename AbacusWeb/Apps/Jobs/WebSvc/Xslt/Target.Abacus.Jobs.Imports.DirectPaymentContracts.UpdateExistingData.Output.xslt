<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Direct Payment Contracts Import - UpdateExistingData Output.
		
		Formats the Target.Abacus.Jobs.Imports.DirectPaymentContracts.UpdateExistingData output information for display.
	-->
	<xsl:template match="Results">
        Total direct payment contracts to process: <xsl:value-of select="TotalToProcess" /><br />
        Total direct payment contracts processed: <xsl:value-of select="TotalProcessed" /><br />
        Total exceptions: <xsl:value-of select="ExceptionsFound" /><br />
        <xsl:if test="number(ExceptionsFound) &gt; 0">
            Exception reports:
            <xsl:if test="string-length(ExceptionCSVFileURL) &gt; 0">
                <a href="{ExceptionCSVFileURL}" target="_blank"> View in Excel (CSV)</a>
            </xsl:if>
            |
            <xsl:if test="string-length(ExceptionsHTML_URL) &gt; 0">
                <a href="{ExceptionsHTML_URL}" target="_blank"> View in XML</a>
            </xsl:if>
        </xsl:if>
    </xsl:template>
	
</xsl:stylesheet>
