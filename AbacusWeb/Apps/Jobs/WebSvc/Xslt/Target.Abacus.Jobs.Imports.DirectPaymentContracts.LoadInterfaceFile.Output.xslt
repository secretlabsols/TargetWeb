<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Direct Payment Contracts Import - LoadInterfaceFile Output.
		
		Formats the Target.Abacus.Jobs.Imports.DirectPaymentContracts.LoadInterfaceFile output information for display.
	-->
	<xsl:template match="Results">
        Total direct payment contracts to import: <xsl:value-of select="TotalToImport" /><br />
        Total direct payment contracts imported: <xsl:value-of select="TotalImported" /><br />
        Total exceptions: <xsl:value-of select="ExceptionsFound" /><br />
        Original file:
        <xsl:choose>
			<xsl:when test="string-length(OriginalXML_URL) = 0"> Not specified</xsl:when>
			<xsl:otherwise>
				<xsl:if test="string-length(OriginalXML_URL) &gt; 0">
					<a href="{OriginalXML_URL}"> View</a>
				</xsl:if>
			</xsl:otherwise>
		</xsl:choose>
        <br />
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
