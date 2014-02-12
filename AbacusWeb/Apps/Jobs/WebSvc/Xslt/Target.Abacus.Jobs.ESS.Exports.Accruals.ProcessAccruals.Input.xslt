<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Accruals Interface - output reports.
		
		Formats the Exports.Accruals.ProcessAccruals output information for display.
	-->
	<xsl:template match="Inputs/FilterAccrualDate">
        Accrual Date:  <xsl:value-of select="." /><br />
	</xsl:template>
    <xsl:template match="Inputs/FilterFinancialYearID">
        Financial Year ID:  <xsl:value-of select="." /><br />
    </xsl:template>
    <xsl:template match="Inputs/FilterPeriodNumberID">
        Period Number ID:  <xsl:value-of select="." /><br />
    </xsl:template>
    <xsl:template match="Inputs/FilterFilePath">
        File Path:  <xsl:value-of select="." /><br />
    </xsl:template>

</xsl:stylesheet>
