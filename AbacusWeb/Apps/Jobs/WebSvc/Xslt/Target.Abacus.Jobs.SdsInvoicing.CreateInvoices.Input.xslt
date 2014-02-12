<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - SDS Invoicing - Create Invoices Input.
		
		Formats the SdsInvoicing.CreateInvoices input information for display.
	-->
	<xsl:template match="Inputs">
        <xsl:choose>
            <xsl:when test="number(Provisional) = 1">Create <strong>provisional</strong> invoices.</xsl:when>
            <xsl:otherwise>Create <strong>actual</strong> invoices.</xsl:otherwise>
        </xsl:choose>
	</xsl:template>
	
</xsl:stylesheet>
