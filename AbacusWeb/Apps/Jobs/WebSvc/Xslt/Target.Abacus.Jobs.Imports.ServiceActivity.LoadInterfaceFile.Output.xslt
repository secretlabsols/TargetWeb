<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Service Activity Import - Load Interface File Output.
		
		Formats the Imports.ServiceActivity.LoadInterfaceFile output information for display.
	-->
	<xsl:template match="Results">
		No. of service activity entries created: <xsl:value-of select="TotalEntries" />
    <br />
		No. of service users: <xsl:value-of select="TotalClients" />
    <br />
    No. of units: <xsl:value-of select="TotalUnits" />
	</xsl:template>
	
</xsl:stylesheet>
