<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Service Order Suspension Import - Update Existing Data Output.
		
		Formats the Imports.DomServiceOrderSuspensions.UpdateExistingData output information for display.
	-->
	<xsl:template match="Results">
		<xsl:value-of select="ExtCount" /> external Suspension record(s) were processed.
		<br />
		<xsl:if test="number(ExceptionsCount) &gt; 0">
			<br /><br />
			<b>IMPORTANT</b>
			<br />
			<xsl:value-of select="ExceptionsCount" /> exception(s) were raised during processing. These suspensions were not processed.
			<br />
			Please see the exceptions report for more details:
			<br />
			View the report <a href="{ExceptionsCsvFileUrl}">in Excel</a>
			<br />
			View the report <a href="{ExceptionsXmlFileUrl}">as XML</a>
		</xsl:if>
	</xsl:template>

</xsl:stylesheet>
