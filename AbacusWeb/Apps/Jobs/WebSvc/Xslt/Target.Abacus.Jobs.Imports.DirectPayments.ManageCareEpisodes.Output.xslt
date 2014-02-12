<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
		Abacus for Windows Job Service - Direct Payments Import - Manage Care Episodes Output.
		
		Formats the Imports.DirectPayments.ManageCareEpisodes output information for display.
	-->
		
	<xsl:template match="Results">
		<xsl:value-of select="PaymentCount" /> net payment agreement(s) processed.
		<br />
		<xsl:value-of select="CreatedCECount" /> new care episode record(s) created.
		<br />
		<xsl:value-of select="FailedToCreateCECount" /> new care episode record(s) could not be created.
		<br />
		<xsl:value-of select="CreatedAssessCount" /> new assessment record(s) created.
		<br />
		<xsl:value-of select="FailedToCreateAssessCount" /> new assessment record(s) could not be created.
		<br />
		<xsl:value-of select="CopiedForwardAssessCount" /> new assessment record(s) copied forward to a new care episode.
		<br />
		<xsl:value-of select="FailedToCopyForwardAssessCount" /> new assessment record(s) could not be copied forward to a new care episode.
		<br />
		<xsl:value-of select="FailedToDeleteCareEpisodeCount" /> new care episode record(s) which failed in later processing could not be deleted.
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
