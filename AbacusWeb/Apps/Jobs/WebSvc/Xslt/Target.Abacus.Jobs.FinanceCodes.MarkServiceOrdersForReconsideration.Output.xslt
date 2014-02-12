<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Mark Service orders to reconsider finance codes
		
		Formats the FinanceCodes.MarkServiceOrdersForReconsideration output information for display.
	-->
    <xsl:template match="Results">
        No. of clients processed: <xsl:value-of select="ClientsProcessed" /><br />
        No. of orders marked for reconsideration: <xsl:value-of select="OrdersMarkedForReconsideration" /><br />

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