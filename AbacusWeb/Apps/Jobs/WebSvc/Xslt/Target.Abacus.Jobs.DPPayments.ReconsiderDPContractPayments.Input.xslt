<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Reconsider DP Contract Payments
		
		Formats the input information for display.
	-->
    <xsl:template match="Inputs">
        <xsl:choose>
            <xsl:when test="number(FilterServiceUserID) &gt; 0">
                Selected Service User: <xsl:value-of select="FilterServiceUserDesc" /><br />
            </xsl:when>
            <xsl:otherwise>
                Selected Service User: (all service users)<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="number(FilterBudgetHolderID) &gt; 0">
                Selected Budget Holder: <xsl:value-of select="FilterBudgetHolderDesc" /><br />
            </xsl:when>
            <xsl:otherwise>
                Selected Budget Holder: (all budget holders)<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="number(FilterBudgetCategoryID) &gt; 0">
                Selected Budget Category: <xsl:value-of select="FilterBudgetCategoryDesc" /><br />
            </xsl:when>
            <xsl:otherwise>
                Selected Budget Category: (all budget categories)<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="contains(FilterDateFrom, '2000')">
                Selected Date From: (open-ended)<br />
            </xsl:when>
            <xsl:when test="string-length(FilterDateFrom) = 0">
                Selected Date From: (open-ended)<br />
            </xsl:when>
            <xsl:otherwise>
                Selected Date From: <xsl:value-of select="FilterDateFrom" /><br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="contains(FilterDateTo, '9999')">
                Selected Date To: (open-ended)<br />
            </xsl:when>
            <xsl:when test="string-length(FilterDateTo) = 0">
                Selected Date To: (open-ended)<br />
            </xsl:when>
            <xsl:otherwise>
                Selected Date To: <xsl:value-of select="FilterDateTo" /><br />
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
</xsl:stylesheet>
