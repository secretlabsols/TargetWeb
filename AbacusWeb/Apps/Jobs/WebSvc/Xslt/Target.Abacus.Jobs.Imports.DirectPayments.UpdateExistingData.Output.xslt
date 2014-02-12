<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Direct Payments Import - Update Existing Data Output.
		
		Formats the Imports.DirectPayments.UpdateExistingData output information for display.
	-->
	<xsl:template match="Results">
		<xsl:value-of select="ExtPaymentCount" /> external payment agreement(s) processed.
        <br />
        <xsl:value-of select="AddedPaymentCount" /> new payment agreement(s) added.
        <br />
		<xsl:value-of select="UpdatedPaymentCount" /> existing payment agreement(s) updated.
        <br />
        <xsl:choose>
            <xsl:when test="number(FailedPaymentCount) &gt; 0">
                <br /><br />
                <b>IMPORTANT</b>
                <br />
                <xsl:value-of select="FailedPaymentCount" /> exception(s) raised during processing. These payment agreements were not processed.
                <br />
                Please see the exceptions report for more details:
                <br />
                View the report <a href="{ExceptionsCSVFileUrl}">in Excel</a>
                <br />
                View the report <a href="{ExceptionsXMLFileUrl}">as XML</a>
            </xsl:when>
            <xsl:otherwise />
        </xsl:choose>
    </xsl:template>
	
</xsl:stylesheet>
