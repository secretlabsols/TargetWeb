<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="no" encoding="UTF-8" />
  <!--
		Abacus for Windows Job Service - Process Visit Amendment Requests
		
		Formats the Imports.DomProforma.ProcessVisitAmendRequests output information for display.
	-->
  <xsl:template match="Results">
    No. of provider invoices retrieved: <xsl:value-of select="FilterInvoicesFound" /><br />
    No. of provider invoices requiring recalculation: <xsl:value-of select="FilterInvoicesRecalced" /><br />
    No. of provider invoices not requiring recalculation: <xsl:value-of select="FilterInvoicesSkipped" />
    <xsl:if test="FilterInvoicesRecalced &gt; 0">
      <br /><br />
      No. of proforma batches created to cover retractions: <xsl:value-of select="ProformaBatchesCreated" /><br />
      No. of retraction proforma invoices created: <xsl:value-of select="ProformaInvsCreated" /><br />
      No. of recalculated provider invoices created: <xsl:value-of select="ProviderInvsCreated" />
    </xsl:if>
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