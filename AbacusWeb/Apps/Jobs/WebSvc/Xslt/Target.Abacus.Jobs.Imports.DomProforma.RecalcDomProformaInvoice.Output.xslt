<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="no" encoding="UTF-8" />
  <!--
		Abacus for Windows Job Service - Domiciliary Pro forma Invoice Recalculation
		
		Formats the Imports.DomProforma.RecalcDomProformaInvoice output information for display.
	-->
  <xsl:template match="Results">
	No. of Payment Schedules retrieved: <xsl:value-of select="FilterPaymentschedulesFound" /><br />
    No. of Pro forma invoices recalculated: <xsl:value-of select="FilterInvoicesRecalced" /><br />
    
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