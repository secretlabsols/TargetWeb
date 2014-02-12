<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Create Domiciliary Provider Invoice From Attendance Register
		
		Formats the DomProviderInvoice.CreateFromAttendanceRegister input information for display.
	-->
    <xsl:template match="Inputs">
        <xsl:choose>
            <xsl:when test="string(NewRegisters)='True'">
                Process new registers: Yes <br />
            </xsl:when>
            <xsl:otherwise>
                Process new registers: No <br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="string(AmendedRegisters)='True'">
                Process amended registers: Yes <br />
            </xsl:when>
            <xsl:otherwise>
                Process amended registers: No <br />
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

    </xsl:template>
</xsl:stylesheet>

