<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Res Care Orders - Reassessments Output.
		
		Formats the Imports.ResCareOrders.Reassessments output information for display.
	-->
	<xsl:template match="Results">
		<xsl:value-of select="Count" /> residential care order reassessment requests were processed from 
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
		<br /><br />
		<xsl:if test="number(CreatedCount) &gt; 0 or number(ReconsideredCount) &gt; 0">
			<xsl:value-of select="CreatedCount" /> new assessment(s) were created.
			<br />
			<xsl:value-of select="ReconsideredCount" /> existing assessment(s) were reconsidered.
			<br />
			Please see the actions report for more details:
			<br />
			View the report <a href="{ActionsCsvFileUrl}">in Excel</a>
			<br />
			View the report <a href="{ActionsXmlFileUrl}">as XML</a>
		</xsl:if>
		<br /><br />
		<xsl:if test="number(ExceptionsCount) &gt; 0">
			<b>IMPORTANT</b>
			<br />
			<xsl:value-of select="ExceptionsCount" /> exception(s) were raised during processing. These requests were not processed.
			<br />
			Please see the exceptions report for more details:
			<br />
			View the report <a href="{ExceptionsCsvFileUrl}">in Excel</a>
			<br />
			View the report <a href="{ExceptionsXmlFileUrl}">as XML</a>
		</xsl:if>
	</xsl:template>
	
</xsl:stylesheet>
