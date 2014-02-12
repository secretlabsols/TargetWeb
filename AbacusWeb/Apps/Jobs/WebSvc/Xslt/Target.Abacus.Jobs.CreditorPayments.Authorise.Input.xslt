<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Authorise Creditor Payments
		
		Formats the input information for display.
	-->
    <xsl:template match="Inputs">
        <xsl:choose>
          <xsl:when test="string-length(FilterPaymentTypesDescription) &gt; 0">
            Selected Types: <xsl:value-of select="FilterPaymentTypesDescription" /><br />
          </xsl:when>
          <xsl:otherwise>
            Selected Types: All<br />
          </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
          <xsl:when test="string-length(FilterNonResidentialFilterDescription) &gt; 0">
            Selected Commitment Indicators: <xsl:value-of select="FilterNonResidentialFilterDescription" /><br />
          </xsl:when>
          <xsl:otherwise>
            Selected Commitment Indicators:<br />
          </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
          <xsl:when test="string-length(FilterPaymentDateRangeDescription) &gt; 0">
            Selected Payment Period: <xsl:value-of select="FilterPaymentDateRangeDescription" /><br />
          </xsl:when>
          <xsl:otherwise>
            Selected Payment Period: All<br />
          </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(FilterGenericCreditorDescription) &gt; 0">
                Selected Creditor: <xsl:value-of select="FilterGenericCreditorDescription" /><br />
            </xsl:when>
            <xsl:otherwise>
                Selected Creditor: All<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(FilterGenericContractDescription) &gt; 0">
                Selected Contract: <xsl:value-of select="FilterGenericContractDescription" /><br />
            </xsl:when>
            <xsl:otherwise>
                Selected Contract: All<br />
            </xsl:otherwise>
        </xsl:choose>
        Selected Excluded: Not Excluded<br />
        Selected Payment Status: Unpaid<br />
        <xsl:choose>
          <xsl:when test="string-length(FilterPaymentStatusDateRangeDescription) &gt; 0">
            Selected Payment Status Date Period: <xsl:value-of select="FilterPaymentStatusDateRangeDescription" /><br />
          </xsl:when>
          <xsl:otherwise>
            Selected Payment Status Date Period: All<br />
          </xsl:otherwise>
        </xsl:choose> 
    </xsl:template>
</xsl:stylesheet>
