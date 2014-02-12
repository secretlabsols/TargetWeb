<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Service Activity Import - Convert Interface File Output.
		
		Formats the Imports.ServiceActivity.ConvertInterfaceFile output information for display.
	-->
	<xsl:template match="Results">
		No. of service activity entries processed: <xsl:value-of select="ActivitiesCount" /><br />
		No. of service users: <xsl:value-of select="UsersCount" /><br />
    No. of units: <xsl:value-of select="UnitsCount" /><br />
    <br />
		<xsl:choose>
			<xsl:when test="string-length(ImportFileURLOrig) = 0 and string-length(ImportFileURLXML) = 0">
        (Import file not specified)<br />
      </xsl:when>
			<xsl:when test="string-length(ImportFileURLOrig) &gt; 0">
        <a href="{ImportFileURLOrig}">View a copy of the original Service Activity file.</a><br />
      </xsl:when>
		</xsl:choose>
		<xsl:if test="string-length(ImportFileURLXML) &gt; 0">
      <a href="{ImportFileURLXML}">View a copy of the Service Activity file after conversion into XML format.</a><br />
    </xsl:if>
    <xsl:if test="string-length(ExcepFileURLXML) &gt; 0">
      <br />
		  No. of exceptions found: <xsl:value-of select="ExcepCount" /><br />
      <br />
      Please see the exceptions report for more details:<br />
      <a href="{ExcepFileURLCSV}">View the report in Excel.</a><br />
      <a href="{ExcepFileURLXML}">View the report as XML.</a>
    </xsl:if>
  </xsl:template>
	
</xsl:stylesheet>
