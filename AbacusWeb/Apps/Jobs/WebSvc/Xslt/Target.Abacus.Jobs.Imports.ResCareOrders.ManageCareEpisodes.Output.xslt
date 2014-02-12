<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
		Abacus for Windows Job Service - Res Care Orders Import - Manage Care Episodes Output.
		
		Formats the Imports.ResCareOrders.ManageCareEpisodes output information for display.
	-->
		
	<xsl:template match="Results">
		<xsl:value-of select="OrderCount" /> residential care order record(s) were processed.
		<br />
		<xsl:value-of select="EndedCount" /> care episode(s) were ended.
		<br />
		<xsl:value-of select="FailedToEndCount" /> care episode(s) could not be ended.
		<br />
		<xsl:value-of select="ReopenCount" /> care episode(s) were re-opened.
		<br />
		<xsl:value-of select="FailedToReopenCount" /> care episode(s) could not be re-opened.
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
		<xsl:value-of select="UpdatedHomeCostCount" /> assessment record(s) were updated with a new home cost.
		<br />
		<xsl:value-of select="FailedToUpdateHomeCostCount" /> assessment record(s) could not be updated with a new home cost.
		<br />
		<xsl:value-of select="FailedToDeleteCareEpisodeCount" /> care episode record(s) which were created but failed in later processing and could not be deleted.
		<br />
		<xsl:value-of select="UpdatedFeeCodeCount" /> care episode record(s) were updated with a new fee code.
		<br />
		<xsl:value-of select="FailedToUpdateFeeCodeCount" /> care episode record(s) could not be updated with a fee code.
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
