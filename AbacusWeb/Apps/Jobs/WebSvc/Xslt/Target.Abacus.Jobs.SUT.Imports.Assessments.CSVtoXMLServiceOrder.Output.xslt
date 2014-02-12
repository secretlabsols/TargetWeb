<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="no" encoding="UTF-8" />
  <!--
		Abacus for Windows Job Service - SUT Import - CSV to XML (Service Orders) output XSLT.
		
		Formats the SUT.Imports.Assessments.CSVtoXMLServiceOrder output information for display.
	-->
  <xsl:template match="Results">
    Conversion of file from CSV to XML successful?: <xsl:value-of select="FileConverted" /><br /><br />
    Original file selected:
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
    <xsl:if test="string-length(ActionsCSVFileURL) &gt; 0">
      <br /><br />
      Exceptions were found with the input file provided. Please see the actions report for more details:
      <br />
      View the report <a href="{ActionsCSVFileURL}">in Excel</a>
      <br />
      View the report <a href="{ActionsXMLFileURL}">as XML</a>
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>