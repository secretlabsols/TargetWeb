<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Financial Export Interface - Apply Xslt Input.
		
		Formats the Exports.FinancialExportInterface.ApplyXslt input information for display.
	-->
	
	<xsl:template match="InterfaceLogID">
        Creditor Payment Batch ID: <xsl:value-of select="." /><br />
	</xsl:template>

    <xsl:template match="InterfaceLogRef">
        Creditor Payment Batch Reference: <xsl:value-of select="." /><br />
    </xsl:template>

    <xsl:template match="XmlTransformID">
        Transform ID: <xsl:value-of select="." /><br />
    </xsl:template>

    <xsl:template match="XmlTransformRef">
        Transform Reference: <xsl:value-of select="." /><br />
    </xsl:template>
    
</xsl:stylesheet>
