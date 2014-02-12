<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
		Abacus for Windows Job Service - Establishments Import - Update Existing Data Output.
		
		Formats the Imports.Establishments.UpdateExistingData output information for display.
	-->
	
	<xsl:template match="Results">
		<xsl:value-of select="ExtEstablishmentCount" /> external establishment record(s) were processed.
		<br />
		<xsl:value-of select="UpdatedEstablishmentCount" /> existing establishment record(s) were updated.
		<br /><br />
		<xsl:if test="number(ExceptionsCount) &gt; 0">
			<b>IMPORTANT</b>
			<br />
			<xsl:value-of select="ExceptionsCount" /> exception(s) were raised during processing. These establishment were not processed.
			<br />
			Please see the exceptions report for more details:
			<br />
			View the report <a href="{ExceptionsCsvFileUrl}">in Excel</a>
			<br />
			View the report <a href="{ExceptionsXmlFileUrl}">as XML</a>
		</xsl:if>
	</xsl:template>
	
</xsl:stylesheet>
