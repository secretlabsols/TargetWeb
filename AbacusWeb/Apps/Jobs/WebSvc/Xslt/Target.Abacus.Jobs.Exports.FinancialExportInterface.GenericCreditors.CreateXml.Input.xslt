<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Generic Creditors Export - Create Xml Input.
		
		Formats the Exports.FinancialExportInterface.GenericCreditors.CreateXml input information for display.
	-->

    <xsl:template match="/">

        <xsl:apply-templates select="//FilterInterfaceLogID" />
        <xsl:apply-templates select="//FilterInterfaceLogBatchReference" />
        <xsl:apply-templates select="//FilterRecreate" />
        <xsl:apply-templates select="//FilterReread" />
        <xsl:apply-templates select="//FilterPostingDate" />
        <xsl:apply-templates select="//FilterPostingYear" />
        <xsl:apply-templates select="//FilterPeriodNumber" />
        <xsl:apply-templates select="//FilterRollbackMethod" />

    </xsl:template>
	
	<xsl:template match="FilterInterfaceLogID">
        Creditor Payment Batch ID: <xsl:value-of select="." /><br />
	</xsl:template>

    <xsl:template match="FilterInterfaceLogBatchReference">
        Creditor Payment Batch Reference: <xsl:value-of select="." /><br />
    </xsl:template>
	
    <xsl:template match="FilterRecreate">
        Recreate?:
        <xsl:choose>
            <xsl:when test="number(.) = 1">Yes</xsl:when>
            <xsl:otherwise>No</xsl:otherwise>
        </xsl:choose>
        <br />
    </xsl:template>

    <xsl:template match="FilterReread">
        Reread?:
        <xsl:choose>
            <xsl:when test="number(.) = 1">Yes</xsl:when>
            <xsl:otherwise>No</xsl:otherwise>
        </xsl:choose>
        <br />
    </xsl:template>

    <xsl:template match="FilterPostingDate">
      Posting Date: <xsl:value-of select="." /><br />
    </xsl:template>

    <xsl:template match="FilterPostingYear">
      Posting Year: <xsl:value-of select="." /><br />
    </xsl:template>

    <xsl:template match="FilterPeriodNumber">
      Period Number: <xsl:value-of select="." /><br />
    </xsl:template>

    <xsl:template match="FilterRollbackMethod">
        Rollback Options:
        <xsl:choose>
            <xsl:when test="number(.) = 0">Do not rollback Batch</xsl:when>
            <xsl:when test="number(.) = 1">Remove erroneous items from the batch</xsl:when>
            <xsl:when test="number(.) = 2">Rollback entire batch upon encountering an error</xsl:when>
        </xsl:choose>
        <br />
    </xsl:template>
	
</xsl:stylesheet>
