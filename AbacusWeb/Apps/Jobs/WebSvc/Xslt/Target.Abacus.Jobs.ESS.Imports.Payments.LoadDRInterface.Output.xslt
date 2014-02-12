<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - ESS Payments Import - Load DR Interface File Output.
		
		Formats the ESS.Imports.Payments.LoadDRInterface output information for display.
	-->
	<xsl:template match="Results">
		<xsl:value-of select="Count" /> Payments were processed from
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
