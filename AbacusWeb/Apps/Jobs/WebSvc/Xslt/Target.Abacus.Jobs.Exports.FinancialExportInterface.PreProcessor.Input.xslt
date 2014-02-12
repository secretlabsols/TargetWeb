<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Financial Export Interface - Pre-processor Input.
		
		Formats the Exports.FinancialExportInterface.PreProcessor input information for display.
	-->
	<xsl:template match="InterfaceLogID">
        Batch ID: <xsl:value-of select="." /><br />
	</xsl:template>

    <xsl:template match="InterfaceLogRef">
        Batch Reference: <xsl:value-of select="." /><br />
    </xsl:template>
	
    <xsl:template match="PreProcessorType">
        Pre-processor:
        <xsl:choose>
            <xsl:when test="string-length(.) = 0">
                (not specified - this step will be skipped)
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="."/>
            </xsl:otherwise>
        </xsl:choose>
        <br />
    </xsl:template>
    
</xsl:stylesheet>
