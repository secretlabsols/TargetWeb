<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Dom Service Orders - Recalculation.
		
		Formats the DomServiceOrders.RecalculateDSO input information for display.
	-->
	<xsl:template match="Inputs">
        <b>Selected contract: </b><xsl:value-of select="ContractDesc" /><br />
	</xsl:template>
	
</xsl:stylesheet>
