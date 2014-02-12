<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Debtors Interface - Create Recurring Output.
		
		Formats the Exports.FinancialExportInterface.Debtors.CreateRecurring output information for display.
	-->
	<xsl:template match="Results">
        <xsl:value-of select="concat('Interface Batch ID: ', InterfaceID)" />
        <br />
        <xsl:value-of select="concat('Interface Batch Ref: ', InterfaceBatchRef)" />
        <br />
        <xsl:value-of select="concat('Job ID: ', JobID)" />
        <br />
        <xsl:value-of select="concat('Job Start Date: ', JobStartDate)" />
        <br />
    </xsl:template>
	
</xsl:stylesheet>
