<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ext="urn:ext-util">
    <xsl:output method="html" indent="no" encoding="ISO-8859-1" />
    <!--
		Abacus for Windows Job Service - Publishing Interface
		
		Formats the Publishing.ProduceDebtorInvoices output information for display.
	-->
	<xsl:template match="Results">
    No. of Invoices sent for publishing: <b><xsl:value-of select="InvsProcessedTotal" /> (<xsl:value-of select="InvsProcessedRes" /> Residential, <xsl:value-of select="InvsProcessedDom" /> Non-Residential)</b><br />
    Residential Invoices published: <b><xsl:value-of select="InvsProcessedRes" /> (<xsl:value-of select="InvSuccessRes" /> successfully, <xsl:value-of select="InvFailedRes" /> unsuccessfully)</b><br />
    Non-Residential Invoices published: <b><xsl:value-of select="InvsProcessedDom" /> (<xsl:value-of select="InvSuccessDom" /> successfully, <xsl:value-of select="InvFailedDom" /> unsuccessfully)</b><br />

    <xsl:if test="number(ExceptionsCount) &gt; 0">
      <br />
      <b>
        <xsl:value-of select="ExceptionsCount" /> exception(s) found during Invoice publishing.<br />
      </b>
      <br />
      <xsl:if test="string-length(ExceptionsCsvFileUrl) &gt; 0">
        Please see the exceptions report for more details:
        <br />
        View the report <a href="{ExceptionsCsvFileUrl}">in Excel</a>
        <br />
        View the report <a href="{ExceptionsXmlFileUrl}">as XML</a>
      </xsl:if>
    </xsl:if>
  </xsl:template>
	
</xsl:stylesheet>
