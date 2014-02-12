<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
  
		Abacus for Windows Job Service - Reports - Export to CSV (OUTPUTS)
    
	-->
		
	<xsl:template match="Results">    
    <xsl:if test="string-length(ExportedReportUrl) &gt; 0">
      Expiry Date: <xsl:value-of select="ExportedReportExpiryDate"/>
      <br />
      Report: <a href="{ExportedReportUrl}">Download</a>
      <br />
      Row Count: <xsl:value-of select="ExportedReportRowCount"/>
      <br />      
    </xsl:if>
    <xsl:if test="string-length(ExportedReportUrl) = 0">
      Export failed, please refer to the 'Progress' tab for futher details.
    </xsl:if>
    <br />
    <br />
	</xsl:template>
	
</xsl:stylesheet>
