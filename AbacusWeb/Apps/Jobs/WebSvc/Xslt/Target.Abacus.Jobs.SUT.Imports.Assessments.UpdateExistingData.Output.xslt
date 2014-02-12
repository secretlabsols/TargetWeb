<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="no" encoding="UTF-8" />
  <!--
		Abacus for Windows Job Service - SUT Import - Update Assessments.
		
		Formats the SUT.Imports.Assessments.UpdateExistingData output information for display.
	-->
  <xsl:template match="Results">
    No. of assessments found to be processed: <xsl:value-of select="AssessmentsFound" /><br />
    No. of assessments successfully uploaded: <xsl:value-of select="AssessmentsLoaded" /><br />
    No. of assessments not successfully uploaded: <xsl:value-of select="AssessmentsExceptions" /><br />
    <xsl:if test="number(AssessmentsFound) &gt; 0">
      <xsl:if test="number(AssessmentsExceptions) &gt; 0">
        <br />
        <b>IMPORTANT</b>
        <br />
        <xsl:value-of select="AssessmentsExceptions" /> exception(s) were raised during processing. These assessments were not processed.
        <br />
      </xsl:if>
      Please see the actions report for more details:
      <br />
      View the report <a href="{ExceptionsCsvFileUrl}">in Excel</a>
      <br />
      View the report <a href="{ExceptionsXmlFileUrl}">as XML</a>
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>