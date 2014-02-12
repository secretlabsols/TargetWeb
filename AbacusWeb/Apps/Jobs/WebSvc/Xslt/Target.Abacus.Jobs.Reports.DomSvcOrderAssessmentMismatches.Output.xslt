<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
		Abacus for Windows Job Service - Reports - Res Care Order/Care Episode Mismatches Output.
		
		Formats the Reports.ResCareOrderCareEpisodeMismatchess output information for display.
	-->
		
	<xsl:template match="Results">
		<xsl:value-of select="MismatchCount" /> mismatches between domiciliary service orders and assessments were found.
		<br /><br />
		<xsl:if test="number(MismatchCount) &gt; 0">
			Please see the report for more details:
			<br />
			View the report <a href="{ReportCsvFileUrl}">in Excel</a>
			<br />
			View the report <a href="{ReportXmlFileUrl}">as XML</a>
		</xsl:if>
	</xsl:template>
	
</xsl:stylesheet>
