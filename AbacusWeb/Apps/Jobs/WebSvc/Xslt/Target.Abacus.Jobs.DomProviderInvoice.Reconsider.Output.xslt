<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Reconsider Domiciliary Provider Invoices
		
		Formats the DomProviderInvoice.Reconsider output information for display.
	-->
    <xsl:template match="Results">
        No. of provider invoices retrieved: <xsl:value-of select="InvoicesFound" /><br />
        No. of provider invoices successfully reconsidered: <xsl:value-of select="InvoicesReconsidered" /><br />
        No. of provider invoices failing creation: <xsl:value-of select="InvoicesFailed" /><br />
        No. of provider invoices skipping creation: <xsl:value-of select="InvoicesSkipped" /><br />
        <xsl:if test="number(ExceptionsCount) &gt; 0">
            <br /><br />
            <b>IMPORTANT</b>
            <br />
            <xsl:value-of select="ExceptionsCount" /> exception(s) were raised during processing. These domiciliary provider invoices were not processed.
            <br />
            Please see the exceptions report for more details:
            <br />
            View the report <a href="{ExceptionsCsvFileUrl}">in Excel</a>
            <br />
            View the report <a href="{ExceptionsXmlFileUrl}">as XML</a>
        </xsl:if>
        <xsl:if test="string-length(ActionsCSVFileURL) &gt; 0">
            <br /><br />
            <b>The following invoices were successfully reconsidered</b>
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
