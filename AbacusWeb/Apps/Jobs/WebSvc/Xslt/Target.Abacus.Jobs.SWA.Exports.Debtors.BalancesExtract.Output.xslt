<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="no" encoding="UTF-8" />
  <!--
		Abacus for Windows Job Service - SWA O/S Invoice Balances extract
		
		Formats the SWA.Exports.Debtors.BalancesExtract output information for display.
	-->
  <xsl:template match="Results">
    No. of invoices found initially: <xsl:value-of select="NumInvoicesFound" /><br />
    No. of invoices successfully extracted: <xsl:value-of select="NumInvoicesIncluded" /><br />
    No. of invoices logged with exceptions: <xsl:value-of select="NumInvoicesExcluded" />
    <xsl:if test="number(NumInvoicesIncluded) &gt; 0">
      <br /><br />
      <b>The Outstanding Balances report is located below:</b>
      <br />
      <xsl:value-of select="OutputReportPath" />
    </xsl:if>
    <xsl:if test="number(NumInvoicesExcluded) &gt; 0">
      <br /><br />
      <b>The exceptions report is located below:</b>
      <br />
      <xsl:value-of select="OutputExcepPath" />
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>