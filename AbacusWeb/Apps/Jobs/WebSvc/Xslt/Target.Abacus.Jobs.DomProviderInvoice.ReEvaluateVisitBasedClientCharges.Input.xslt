<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Re-Evaluate visit based client charges
		
		Formats the DomProviderInvoice.ReEvaluateVisitBasedClientCharges input information for display.
	-->
    <xsl:template match="Inputs">
        <xsl:choose>
            <xsl:when test="string(CalcMethod)='1'">
                Calculation Method: The lower of Duration Claimed and Duration Paid<br />
            </xsl:when>
            <xsl:when test="string(CalcMethod)='2'">
                Calculation Method: The lowest of Duration Claimed, Duration Paid and Actual Duration<br />
            </xsl:when>
            <xsl:when test="string(CalcMethod)='3'">
                Calculation Method: The Actual Duration<br />
            </xsl:when>
            <xsl:when test="string(CalcMethod)='4'">
                Calculation Method: The Duration Paid<br />
            </xsl:when>
            <xsl:otherwise>
                Calculation Method: None <br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(FilterProviderDesc) &gt; 0">
                Selected provider: <xsl:value-of select="FilterProviderDesc" /><br />
            </xsl:when>
            <xsl:otherwise>
                Selected provider: (all providers)<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(FilterContractDesc) &gt; 0">
                Selected contract: <xsl:value-of select="FilterContractDesc" /><br />
            </xsl:when>
            <xsl:otherwise>
                Selected contracts: (all contracts)<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(FilterDateFrom) &gt; 0">
                Date From: <xsl:value-of select="FilterDateFrom" /><br />
            </xsl:when>
            <xsl:otherwise>
                Date From: (No Filter)<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(FilterDateTo) &gt; 0">
                Date To: <xsl:value-of select="FilterDateTo" /><br />
            </xsl:when>
            <xsl:otherwise>
                Date To: (No Filter)<br />
            </xsl:otherwise>
        </xsl:choose>       
        
        
    </xsl:template>
</xsl:stylesheet>


