<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Debtors Interface - Apply Xslt Input.
		
		Formats the Exports.Debtors.ApplyOT09Xslt input information for display.
	-->
	
	<xsl:template match="InterfaceLogID">
        Debtors Invoice Batch ID: <xsl:value-of select="." /><br />
	</xsl:template>

    <xsl:template match="InterfaceLogRef">
        Debtors Invoice Batch Reference: <xsl:value-of select="." /><br />
    </xsl:template>

    <xsl:template match="XmlTransformID">
        Transform ID: <xsl:value-of select="." /><br />
    </xsl:template>

    <xsl:template match="XmlTransformRef">
        Transform Reference: <xsl:value-of select="." /><br />
    </xsl:template>
    
</xsl:stylesheet>
