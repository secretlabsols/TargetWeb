<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Debtors Interface - Aged Debt By Client Output.
		
		Formats the Exports.Debtors.ReportAgedDebtByClient output information for display.
	-->
	<xsl:template match="Results/OutputFilePath">
        File output to: <xsl:value-of select="." /><br />
	</xsl:template>
    
</xsl:stylesheet>
