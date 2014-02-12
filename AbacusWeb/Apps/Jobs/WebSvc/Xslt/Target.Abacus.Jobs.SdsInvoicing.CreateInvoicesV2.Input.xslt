<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:ext="urn:ext-util" exclude-result-prefixes="ext">
  <!--
		Abacus for Windows Job Service - SDS Invoicing - Create Invoices Input.
		
		Formats the SdsInvoicing.CreateInvoices input information for display.
	-->
  <xsl:template match="Inputs">
    <xsl:choose>
      <xsl:when test="string(Provisional) = 'True'">
        Report Only for 
        <xsl:choose>
          <xsl:when test="string(FilterServiceUserID) = '0'">
            All Clients            
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="string(FilterServiceUserName)" />
          </xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:otherwise>
        Create <strong>actual</strong> invoices for 
        
        <xsl:choose>
          <xsl:when test="string(FilterServiceUserID) = '0'">
            All Clients
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="string(FilterServiceUserName)" />
          </xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>     
    </xsl:choose>
    <br />Invoice up to: <xsl:value-of select="ext:FormatDate(InvoiceUpTo)" />
  </xsl:template>

</xsl:stylesheet>
