<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Domiciliary Provider Invoice Creation
		
		Formats the Imports.DomProforma.CreateDomProviderInvoices output information for display.
	-->
	<xsl:template match="Results">
    No. of batches of pro forma invoices retrieved: <xsl:value-of select="BatchesFound" /><br />
    No. of pro forma invoices retrieved: <xsl:value-of select="InvoicesFound" /><br />
    No. of provider invoices successfully created: <xsl:value-of select="InvoicesCreated" /><br />
    No. of provider invoices failing creation: <xsl:value-of select="InvoicesFailed" /><br />
    No. of provider invoices skipping creation: <xsl:value-of select="InvoicesSkipped" /><br />
    No. of batches marked as invoiced: <xsl:value-of select="BatchesInvoiced" />
  <br /><br />
    <xsl:if test="string-length(ActionsCSVFileURL) &gt; 0">
      Please see the actions report for more details:
      <br />
      View the report <a href="{ActionsCSVFileURL}">in Excel</a>
      <br />
      View the report <a href="{ActionsXMLFileURL}">as XML</a>
    </xsl:if>
  </xsl:template>	
</xsl:stylesheet>