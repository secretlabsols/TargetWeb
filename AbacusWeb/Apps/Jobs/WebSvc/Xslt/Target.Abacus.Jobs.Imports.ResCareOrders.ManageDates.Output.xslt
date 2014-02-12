<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
		Abacus for Windows Job Service - Res Care Orders Import - Manage Dates Output.
		
		Formats the Imports.ResCareOrders.ManageDates output information for display.
	-->
		
	<xsl:template match="Results">
		<xsl:value-of select="OrderCount" /> external residential care order record(s) were processed.
		<br /><br />
		Please see the progress information for more details.
	</xsl:template>
	
</xsl:stylesheet>
