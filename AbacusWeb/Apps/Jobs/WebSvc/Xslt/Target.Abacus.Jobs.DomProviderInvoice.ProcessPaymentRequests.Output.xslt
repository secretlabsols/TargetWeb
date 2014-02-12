<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="no" encoding="UTF-8" />
  <!--
		Abacus for Windows Job Service - Process Payment Requests
		
		Formats the DomProviderInvoice.ProcessPaymentRequests output information for display.
	-->
  <xsl:template match="Results">
    The number of Contracts affected during processing: <xsl:value-of select="ContractsAffected" /><br />
    The number of new Payment Schedules created: <xsl:value-of select="PaymentSchedulesCreated" /><br />
    The number of Pro forma Invoices created: <xsl:value-of select="ProformaInvoicesCreated" /><br />
    <xsl:if test="string-length(ActionsCSVFileURL) &gt; 0">
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
