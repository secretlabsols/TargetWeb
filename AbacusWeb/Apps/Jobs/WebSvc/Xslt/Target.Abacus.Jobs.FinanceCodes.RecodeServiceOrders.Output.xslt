<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Recode Service Order finance codes
		
		Formats the FinanceCodes.RecodeServiceOrders output information for display.
	-->
    <xsl:template match="Results">
        No. of orders processed: <xsl:value-of select="ServiceOrdersReconsidered" /><br />
        No. of orders allocated a finance code: <xsl:value-of select="OrdersAllocatedFinanceCodes" /><br />
        No. of orders NOT allocated a finance code: <xsl:value-of select="OrdersWithUnallocatedFinanceCodes" /><br />

        <xsl:if test="number(ExceptionsCount) &gt; 0">
            <br /><br />
            <b>IMPORTANT</b>
            <br />
            <xsl:value-of select="ExceptionsCount" /> exception(s) were raised during processing.
            <br />
            Please see the exceptions report for more details:
            <br />
            View the report <a href="{ExceptionsCsvFileUrl}">in Excel</a>
            <br />
            View the report <a href="{ExceptionsXmlFileUrl}">as XML</a>
        </xsl:if>
        <br /><br />
    </xsl:template>
</xsl:stylesheet>
