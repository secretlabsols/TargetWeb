<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="no" encoding="UTF-8" />
  <!--
		Abacus for Windows Job Service - Process Visit Amendment Requests
		
		Formats the Imports.DomProforma.ProcessVisitAmendRequests input information for display.
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
    <br />
    <xsl:choose>
      <xsl:when test="string-length(FilterCreateProforma) = 0 or FilterCreateProforma = 'True'">
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