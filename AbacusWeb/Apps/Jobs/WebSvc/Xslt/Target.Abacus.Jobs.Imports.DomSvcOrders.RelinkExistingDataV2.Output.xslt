<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
		Abacus for Windows Job Service - Dom Svc Orders Import - Relink Existing Data V2 Output.
		
		Formats the Imports.DomSvcOrders.RelinkExistingDataV2 output information for display.
	-->
		
	<xsl:template match="Results">
		<xsl:value-of select="TotalUpdatedDSAs" /> existing domiciliary service actual record(s) re-linked.
		<br />
		<xsl:value-of select="TotalUpdatedInvDets" /> existing invoice detail record(s) re-linked.
		<br />
	</xsl:template>
	
</xsl:stylesheet>
