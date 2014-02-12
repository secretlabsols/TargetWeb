<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:ext="urn:ext-util" exclude-result-prefixes="ext">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
		Abacus for Windows Job Service - SDS Invoicing - Create Invoices Output.
		
		Formats the SdsInvoicing.CreateInvoices output information for display.
	-->
		
	<xsl:template match="Results">
		<xsl:value-of select="InvoiceCount" /> invoices(s) were created with a total value of <xsl:value-of select="ext:FormatCurrency(TotalValue)" />.
        <xsl:if test="string-length(ReportCsvFileUrl) &gt; 0">
            <br /><br />
			Please see the output report for more details:
			<br />
			View the report <a href="{ReportCsvFileUrl}">in Excel</a>
			<br />
			View the report <a href="{ReportXmlFileUrl}">as XML</a>
		</xsl:if>
	</xsl:template>
	
</xsl:stylesheet>
