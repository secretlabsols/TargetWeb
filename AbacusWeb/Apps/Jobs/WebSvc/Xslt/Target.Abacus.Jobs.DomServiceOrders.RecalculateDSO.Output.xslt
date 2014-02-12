<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
		Abacus for Windows Job Service - Dom Service Orders - Recalculation.
		
		Formats the DomServiceOrders.RecalculateDSO output information for display.
	-->

    <xsl:template match="Results">
        <b>Selected Contract: </b><xsl:value-of select="ContractDetails" />
        <br /><br />
        <xsl:value-of select="FilterDSOsFound" /> related non-residential service order(s) were processed.
        <br />
        <xsl:value-of select="FilterDSOsRecalced" /> order(s) were successfully recalculated.
        <br />
        <xsl:value-of select="FilterDSOsSkipped" /> order(s) were not recalculated.
        <br /><br />
        <b>IMPORTANT</b>
        <br />
        Please see the actions report for more details:
        <br />
        View the report <a href="{ActionsCSVFileURL}">in Excel</a>
        <br />
        View the report <a href="{ActionsXMLFileURL}">as XML</a>
    </xsl:template>

</xsl:stylesheet>
