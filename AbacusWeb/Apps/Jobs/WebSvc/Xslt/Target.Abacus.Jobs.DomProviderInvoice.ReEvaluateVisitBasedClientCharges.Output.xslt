<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Re-Evaluate visit based client charges
		
		Formats the DomProviderInvoice.ReEvaluateVisitBasedClientCharges output information for display.
	-->
    <xsl:template match="Results">
        No. of invoices processed: <xsl:value-of select="InvoicesFound" /><br />
        No. of provider invoices re-evaluated: <xsl:value-of select="InvoicesRe-evaluated" /><br />
        No. of provider invoices that could not be re-evaluated: <xsl:value-of select="InvoicesFailed" /><br />
        <xsl:if test="string-length(ActionsCSVFileURL) &gt; 0">
            <br /><br />
            <b>The following file lists invoices sucessfully re-evaluated and any exceptions</b>
            <br />
            Please see the actions report for more details:
            <br />
            View the report <a href="{ActionsCSVFileURL}">in Excel</a>
            <br />
            View the report <a href="{ActionsXMLFileURL}">as XML</a>
        </xsl:if>
        <br /><br />
    </xsl:template>
</xsl:stylesheet>
