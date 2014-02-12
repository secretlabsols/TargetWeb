<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
		Abacus for Windows Job Service - Dom Svc Orders Import - Reconsider Domiciliary Provider Invoice Status Output.
		
		Formats the Imports.DomSvcOrders.ReconsiderDomProviderInvoiceStatus output information for display.
	-->
		
	<xsl:template match="Results">
		<xsl:value-of select="OrderCount" /> domiciliary service order record(s) were processed.
		<br />
		<xsl:value-of select="InvoicesProcessed" /> domiciliary provider invoice record(s) were processed.
		<br />
		<xsl:value-of select="InvoicesUpdated" /> domiciliary provider invoice record(s) were updated.
        <xsl:if test="string-length(ActionsCsvFileUrl) &gt; 0">
            <br /><br />
			Please see the actions report for more details:
			<br />
			View the report <a href="{ActionsCsvFileUrl}">in Excel</a>
			<br />
			View the report <a href="{ActionsXmlFileUrl}">as XML</a>
		</xsl:if>
	</xsl:template>
	
</xsl:stylesheet>
