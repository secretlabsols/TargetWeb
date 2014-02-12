<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="no" encoding="UTF-8" />
  <!--
		Abacus for Windows Job Service - SWA Import - Load Payments.
		
		Formats the SWA.Imports.Payments.LoadPayments output information for display.
	-->
  <xsl:template match="Results">
    No. of payments found in input file: <xsl:value-of select="PaymentsFound" /><br />
    No. of payments successfully loaded: <xsl:value-of select="PaymentsLoaded" /><br />
    No. of payments successfully loaded (with warnings): <xsl:value-of select="PaymentsLoadedWithWarning" /><br />
    No. of payments logged with exceptions: <xsl:value-of select="PaymentsExceptions" /><br /><br />
    Payment(s) processed from
    <xsl:choose>
      <xsl:when test="string-length(InterfaceFile) = 0">(file not specified)</xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="InterfaceFile" />.
        <br />
        <xsl:if test="string-length(InterfaceFileURL) &gt; 0">
          <a href="{InterfaceFileURL}">View a copy of this file.</a>
        </xsl:if>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:if test="string-length(PostingsCSVFileURL) &gt; 0">
      <br /><br />
      Please see the Transactions Posted report for more details:
      <br />
      View the report <a href="{PostingsCSVFileURL}">in Excel</a>
      <br />
      View the report <a href="{PostingsXMLFileURL}">as XML</a>
    </xsl:if>
    <xsl:if test="string-length(ExceptionsCSVFileURL) &gt; 0">
      <br /><br />
      Please see the exceptions report for more details:
      <br />
      View the report <a href="{ExceptionsCSVFileURL}">in Excel</a>
      <br />
      View the report <a href="{ExceptionsXMLFileURL}">as XML</a>
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>