<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Financial Export Interface - Apply Xslt Output.
		
		Formats the Exports.FinancialExportInterface.ApplyXslt output information for display.
	-->
	<xsl:template match="OutputFilePath">
        File output to: <xsl:value-of select="." /><br />
	</xsl:template>
    
</xsl:stylesheet>
