<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Authorise Domiciliary Provider Invoice
		
		Formats the DomProviderInvoice.Authorise input information for display.
	-->
    <xsl:template match="Inputs">
        <xsl:choose>
            <xsl:when test="string-length(ProviderName) &gt; 0">
                Selected provider: <xsl:value-of select="ProviderName" /><br />
            </xsl:when>
            <xsl:otherwise>
                Selected provider: (all providers)<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(ContractNoTitle) &gt; 0">
                Selected contract: <xsl:value-of select="ContractNoTitle" /><br />
            </xsl:when>
            <xsl:otherwise>
                Selected contracts: (all contracts)<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(ServiceUserName) &gt; 0">
                Selected service user: <xsl:value-of select="ServiceUserName" /><br />
            </xsl:when>
            <xsl:otherwise>
                Selected service user: (all service users)<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:if test="string-length(InvoiceNumber) &gt; 0">
            Invoice Number:
            <xsl:value-of select="InvoiceNumber" />
            <br />
        </xsl:if>
        <xsl:if test="string-length(InvoiceReference) &gt; 0">
            Invoice Reference:
            <xsl:value-of select="InvoiceReference" />
            <br />
        </xsl:if>
        Week Ending Date Range:
        <xsl:if test="string-length(WEDateFrom) &gt; 0">
            From <xsl:value-of select="WEDateFrom" /> 
        </xsl:if>
        <xsl:if test="string-length(WEDateTo) &gt; 0">
            To <xsl:value-of select="WEDateTo" />
        </xsl:if>
        <br />
        <xsl:if test="string-length(InvoiceStatus) &gt; 0">
            Invoice Status:
            <xsl:value-of select="InvoiceStatus" />
            <br />
        </xsl:if>
        Invoice Status Date Range:
        <xsl:if test="string-length(StatusDateFrom) &gt; 0">
            From <xsl:value-of select="StatusDateFrom" />
        </xsl:if>
        <xsl:if test="string-length(StatusDateTo) &gt; 0">
            To <xsl:value-of select="StatusDateTo" />
        </xsl:if>
    </xsl:template>
</xsl:stylesheet>
