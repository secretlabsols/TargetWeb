<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="no" encoding="UTF-8" />
  <!--
		Abacus for Windows Job Service - Domiciliary Provider Invoice Recalculation
		
		Formats the Imports.DomProforma.RecalcDomProviderInvoices input information for display.
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
        Selected invoice start date: <xsl:value-of select="FilterDateFrom" /><br />
      </xsl:when>
      <xsl:otherwise>
        Selected invoice start date: (no limit)<br />
      </xsl:otherwise>
    </xsl:choose>
    <xsl:choose>
      <xsl:when test="string-length(FilterDateTo) &gt; 0">
        Selected invoice end date: <xsl:value-of select="FilterDateTo" /><br />
      </xsl:when>
      <xsl:otherwise>
        Selected invoice end date: (no limit)<br />
      </xsl:otherwise>
    </xsl:choose>
    <br />
    <xsl:choose>
      <xsl:when test="FilterCreateProforma = 'True'">
        Create recalculated proforma invoices?: YES<br />
      </xsl:when>
      <xsl:otherwise>
        Create recalculated proforma invoices?: NO<br />
      </xsl:otherwise>
    </xsl:choose>
    <xsl:choose>
      <xsl:when test="FilterCreateProvider = 'True'">
        Create recalculated provider invoices?: YES<br />
      </xsl:when>
      <xsl:otherwise>
        Create recalculated provider invoices?: NO<br />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>