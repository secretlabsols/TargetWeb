<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
		Abacus for Windows Job Service - Res CCE Orders Import - Manage Assessed Cost Elements Output.
		
		Formats the Imports.ResCCEOrders.ManageAssessedCostElements output information for display.
	-->
		
	<xsl:template match="Results">
		<xsl:value-of select="AssessmentsCount" /> assessment record(s) were processed.
		<br />
		<xsl:value-of select="SuccessCount" /> were successful.
		<br />
		<xsl:value-of select="FailureCount" /> failed.
		<br /><br />
		<xsl:if test="string-length(ActionsCsvFileUrl) &gt; 0">
			Please see the actions report for more details:
			<br />
			View the report <a href="{ActionsCsvFileUrl}">in Excel</a>
			<br />
			View the report <a href="{ActionsXmlFileUrl}">as XML</a>
		</xsl:if>
	</xsl:template>
	
</xsl:stylesheet>
