<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Dom Creditors Export - Create Xml Input.
		
		Formats the Exports.FinancialExportInterface.DomCreditors.CreateXml input information for display.
	-->
	<xsl:template match="InterfaceLogID">
        Domiciliary Provider Invoice Batch ID: <xsl:value-of select="." /><br />
	</xsl:template>

    <xsl:template match="InterfaceLogRef">
        Domiciliary Provider Invoice Batch Reference: <xsl:value-of select="." /><br />
    </xsl:template>
	
    <xsl:template match="RegenerateXml">
        Regenerate Xml Data: 
        <xsl:choose>
            <xsl:when test="number(.) = 1">Yes</xsl:when>
            <xsl:otherwise>No</xsl:otherwise>
        </xsl:choose>
        <br />
    </xsl:template>

    <xsl:template match="RollbackOptions">
        Rollback Options:
        <xsl:choose>
            <xsl:when test="number(.) = 0">Do not rollback Batch</xsl:when>
            <xsl:when test="number(.) = 1">Partially rollback Batch upon encountering one or more error</xsl:when>
            <xsl:when test="number(.) = 2">Rollback entire Batch upon encountering one or more error</xsl:when>
        </xsl:choose>
        <br />
    </xsl:template>
	
</xsl:stylesheet>
