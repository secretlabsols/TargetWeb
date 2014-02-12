<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="html" indent="no" encoding="UTF-8" />

	<!--
		Abacus for Windows Job Service - Dom Service Orders - Recalculation.
		
		Formats the DomServiceOrders.RecalculateDSO output information for display.
	-->

	<xsl:template match="Results">
		<b>Selected Service User: </b>
		<xsl:choose>
			<xsl:when test="string-length(FilterServiceUser) &gt; 0">
				<xsl:value-of select="FilterServiceUser" />
			</xsl:when>
			<xsl:otherwise>
				All Service Users
			</xsl:otherwise>
		</xsl:choose>
		<br /><br />
		<b>No. of service user weeks processed: </b><xsl:value-of select="NoActualsProcessed" />
		<br />
		<b>No. of service user weeks recalculated: </b><xsl:value-of select="NoActualsRecalculated" />
		<br />
		<br />
		<xsl:if test="string-length(ActionsCSVFileURL) &gt; 0">
			Please see the actions report for more details:
			<br />
			View the report <a href="{ActionsCSVFileURL}">in Excel</a>
			<br />
			View the report <a href="{ActionsXMLFileURL}">as XML</a>
		</xsl:if>
	</xsl:template>

</xsl:stylesheet>
