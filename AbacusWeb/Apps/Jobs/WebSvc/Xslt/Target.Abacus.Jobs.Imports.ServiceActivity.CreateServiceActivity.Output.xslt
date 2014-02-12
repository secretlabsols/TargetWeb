<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
		Abacus for Windows Job Service - Service Activity Import - Create Service Activity Output.
		
		Formats the Imports.ServiceActivity.CreateServiceActivity output information for display.
	-->
		
	<xsl:template match="Results">
		<xsl:value-of select="TotalEntries" /> service activity entry record(s) were processed.
		<br />
		<xsl:value-of select="TotalClients" /> unique service user(s) were processed.
		<br />
		<xsl:value-of select="TotalUnits" /> unit(s) of service were processed.		
		<xsl:if test="number(TotalExceptions) &gt; 0">
			<br /><br />
			<xsl:value-of select="TotalExceptions" /> exception(s) were found during processing.
			<br /><br />
			Please see the exceptions report for more details:
			<br />
			View the report <a href="{ExceptionsCSVFileURL}">in Excel</a>
			<br />
			View the report <a href="{ExceptionsXMLFileURL}">as XML</a>
		</xsl:if>
	</xsl:template>
	
</xsl:stylesheet>
