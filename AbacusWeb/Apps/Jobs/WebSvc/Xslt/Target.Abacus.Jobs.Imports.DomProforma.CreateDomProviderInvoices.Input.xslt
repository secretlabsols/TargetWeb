<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Domiciliary Provider Invoice Creation
		
		Formats the Imports.DomProforma.CreateDomProviderInvoices input information for display.
	-->
	<xsl:template match="/">
    <xsl:choose>
        <xsl:when test="string-length(FilterBatchTypeDesc) &gt; 0">
            Selected batch type(s): <xsl:value-of select="FilterBatchTypeDesc" /><br />
        </xsl:when>
        <xsl:otherwise>
            Selected batch type(s): (all batch types)<br />
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
    <xsl:when test="string-length(FilterContractType) &gt; 0">
      Selected contract type: <xsl:value-of select="FilterContractType" /><br />
    </xsl:when>
    <xsl:otherwise>
      Selected contract type: (all contract types)<br />
    </xsl:otherwise>
  </xsl:choose>
  <xsl:choose>
    <xsl:when test="string-length(FilterContractGroupDesc) &gt; 0">
      Selected contract group: <xsl:value-of select="FilterContractGroupDesc" /><br />
    </xsl:when>
    <xsl:otherwise>
      Selected contract group: (all contract groups)<br />
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
	</xsl:template>	
</xsl:stylesheet>