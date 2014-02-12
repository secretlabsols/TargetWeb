<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
		Abacus for Windows Job Service - Dom Svc Orders Import - Manage Care Episodes Output.
		
		Formats the Imports.DomSvcOrders.ManageCareEpisodes output information for display.
	-->
		
	<xsl:template match="Results">
		<xsl:value-of select="OrderCount" /> domiciliary service order record(s) were processed.
		<br />
		<xsl:value-of select="CreatedCECount" /> new care episode record(s) were created.
		<br />
		<xsl:value-of select="FailedToCreateCECount" /> new care episode record(s) could not be created.
		<br />
		<xsl:value-of select="CreatedAssessCount" /> new assessment record(s) were created.
		<br />
		<xsl:value-of select="FailedToCreateAssessCount" /> new assessment record(s) could not be created.
		<br />
		<xsl:value-of select="CopiedForwardAssessCount" /> new assessment record(s) were copied forward to a new care episode.
		<br />
		<xsl:value-of select="FailedToCopyForwardAssessCount" /> new assessment record(s) could not be copied forward to a new care episode.
		<br />
		<xsl:value-of select="FailedToDeleteCareEpisodeCount" /> care episode record(s) which were created but failed in later processing and could not be deleted.
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
