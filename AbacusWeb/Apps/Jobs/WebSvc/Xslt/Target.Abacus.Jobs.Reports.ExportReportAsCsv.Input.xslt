<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
  
		Abacus for Windows Job Service - Reports - Export to CSV (INPUTS)
    
	-->
		
	<xsl:template match="Inputs">
    Report: <xsl:value-of select="FilterReportCsvExporterReportDescription" />
    <br />
    User: <xsl:value-of select="FilterReportCsvExporterUserName" />
    <br />
    User Email: <xsl:value-of select="FilterReportCsvExporterUserEmailAddress" />
    <br />
		<br /><br />
	</xsl:template>
	
</xsl:stylesheet>
