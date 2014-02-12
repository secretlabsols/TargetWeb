<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:ext="urn:ext-util" exclude-result-prefixes="ext">
	
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Service Commitments Interface
		
		Formats the Commitments.GenerateData input information for display.
	-->
    <xsl:template match="Inputs">
        Include Commitments To: <xsl:value-of select="ext:FormatDateCustom(string(FilterCommitmentDate), 'dd/MM/yyyy')" />
        <br />
        <xsl:choose>
            <xsl:when test="string-length(FilterResidential) &gt; 0 and FilterResidential = 'True'">
                Include Residential Care?: Yes<br />
            </xsl:when>
            <xsl:otherwise>
                Include Residential Care?: No<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(FilterDirectPayments) &gt; 0 and FilterDirectPayments = 'True'">
                Include Direct Payments?: Yes<br />
            </xsl:when>
            <xsl:otherwise>
                Include Direct Payments?: No<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(FilterIncludeLastRun) &gt; 0 and FilterIncludeLastRun = 'True'">
                Include Negated Commitments from previous run?: Yes<br />
            </xsl:when>
            <xsl:otherwise>
                Include Negated Commitments from previous run?: No<br />
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
</xsl:stylesheet>