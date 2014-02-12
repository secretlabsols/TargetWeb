<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Direct Payments Import - Load Interface File Output.
		
		Formats the Imports.DirectPayments.LoadInterfaceFile output information for display.
	-->
	<xsl:template match="Results">
		<xsl:value-of select="PaymentCountTotal" /> payment agreement(s) processed from 
		<xsl:choose>
			<xsl:when test="string-length(InterfaceFile) = 0">(file not specified)</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="InterfaceFile" />.
				<br />
				<xsl:if test="string-length(InterfaceFileURL) &gt; 0">
					<a href="{InterfaceFileURL}">View a copy of this file.</a>
				</xsl:if>
			</xsl:otherwise>
		</xsl:choose>
    </xsl:template>
	
</xsl:stylesheet>
