<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Managing SDS Transactions
		
		Formats the output for the step
	-->
    <xsl:template match="Results">
    
        Total service users reconsidered: <xsl:value-of select="TotalNumberOfServiceUsersReconsidered" /><br />
        Total service user budget periods reconsidered: <xsl:value-of select="TotalNumberOfClientBudgetPeriodsReconsidered" /><br />        
        Total exceptions: <xsl:value-of select="TotalExceptions" /><br />
        Total warnings: <xsl:value-of select="TotalWarnings" /><br />
           
        <xsl:if test="TotalExceptions &gt; 0">
            <br /><br />
            Please see the exceptions report for more details:
            <br />
            View the report <a href="{ExceptionCSVFileURL}">in Excel</a>
            <br />
            View the report <a href="{ExceptionXMLFileURL}">as XML</a>
        </xsl:if>

        <xsl:if test="TotalWarnings &gt; 0">
            <br /><br />
            Please see the warnings report for more details:
            <br />
            View the report <a href="{WarningCSVFileURL}">in Excel</a>
            <br />
            View the report <a href="{WarningXMLFileURL}">as XML</a>
        </xsl:if>

    </xsl:template>
</xsl:stylesheet>