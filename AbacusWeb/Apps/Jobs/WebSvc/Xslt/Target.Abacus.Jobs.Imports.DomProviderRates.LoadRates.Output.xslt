<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
		Abacus for Windows Job Service - Domiciliary Provider Rates Import - Load Rates Output.
		
		Formats the Imports.DomProviderRates.LoadRates output information for display.
	-->
		
	<xsl:template match="Results">
		<xsl:value-of select="DeletedCount" /> existing domiciliary provider rate record(s) were deleted.
		<br />
		<xsl:value-of select="ProcessedCount" /> new domiciliary provider rate record(s) were processed from
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
		<br />
		<xsl:value-of select="FailedToCreateCount" /> new domiciliary provider rate record(s) could not be created.
		<br /><br />
		<xsl:if test="string-length(ExceptionsCsvFileUrl) &gt; 0">
			Please see the exceptions report for more details:
			<br />
			View the report <a href="{ExceptionsCsvFileUrl}">in Excel</a>
			<br />
			View the report <a href="{ExceptionsXmlFileUrl}">as XML</a>
		</xsl:if>
	</xsl:template>
	
</xsl:stylesheet>
