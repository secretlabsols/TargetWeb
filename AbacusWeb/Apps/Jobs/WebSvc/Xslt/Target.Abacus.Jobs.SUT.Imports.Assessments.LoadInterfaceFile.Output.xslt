<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="no" encoding="UTF-8" />
  <!--
		Abacus for Windows Job Service - SUT Import - Load Assessments.
		
		Formats the SUT.Imports.Assessments.LoadInterfaceFile output information for display.
	-->
  <xsl:template match="Results">
    No. of assessments found in input file: <xsl:value-of select="NetIncomesFound" /><br />
    No. of assessments successfully processed: <xsl:value-of select="NetIncomesLoaded" /><br />
    No. of assessments unsuccessfully processed: <xsl:value-of select="NetIncomesNotLoaded" /><br />
    No. of assessments found with exceptions: <xsl:value-of select="NetIncomesInvalid" /><br /><br />
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
      Please see the actions report for more details:
      <br />
      View the report <a href="{ActionsCSVFileURL}">in Excel</a>
      <br />
      View the report <a href="{ActionsXMLFileURL}">as XML</a>
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>