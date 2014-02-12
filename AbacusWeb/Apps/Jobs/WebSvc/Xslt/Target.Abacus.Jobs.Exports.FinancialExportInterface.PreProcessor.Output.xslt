<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Financial Export Interface - Pre-processor Output.
		
		Formats the Exports.FinancialExportInterface.PreProcessor output information for display.
	-->
	<xsl:template match="Results/OriginalXmlFileUrl">
        <a href="{.}">View a copy of the original XML file before processing.</a>
        <br />
	</xsl:template>

    <xsl:template match="Results/WorkingXmlFileUrl">
        <a href="{.}">View a copy of the modified XML file after processing.</a>
        <br />
    </xsl:template>
    
</xsl:stylesheet>
