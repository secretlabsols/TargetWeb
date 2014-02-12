<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="no" encoding="UTF-8" />
  <!--
		Abacus for Windows Job Service - Domiciliary Proforma Invoice Recalculation
		
		Formats the Imports.DomProforma.RecalcDomProformaInvoice input information for display.
	-->
  <xsl:template match="Inputs">
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
        Selected week from: <xsl:value-of select="FilterDateFrom" /><br />
      </xsl:when>
      <xsl:otherwise>
        Selected week from: (no limit)<br />
      </xsl:otherwise>
    </xsl:choose>
    <xsl:choose>
      <xsl:when test="string-length(FilterDateTo) &gt; 0">
        Selected week to: <xsl:value-of select="FilterDateTo" /><br />
      </xsl:when>
      <xsl:otherwise>
        Selected week to: (no limit)<br />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>