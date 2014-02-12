<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Create DP Payments
		
		Formats the output for the step
	-->
    <xsl:template match="Results">
    
        No. of payments to process: <xsl:value-of select="TotalPaymentToProcess" /><br />
        No. of payments processed: <xsl:value-of select="TotalPaymentsProcessed" /><br />
        Total gross payments: <xsl:value-of select="TotalGrossPaymentsAmount" /><br />
        Total service user contributions: <xsl:value-of select="TotalServiceUserContributionsAmount" /><br />
        Total net payments: <xsl:value-of select="TotalNetPaymentsAmount" /><br />        
        Total exceptions: <xsl:value-of select="TotalExceptions" /><br />
        Total warnings: <xsl:value-of select="TotalWarnings" /><br />
        
        <xsl:if test="string-length(ReportCSVFileURL) &gt; 0">
            <br /><br />
            Please see the payments report for more details:
            <br />
            View the report <a href="{ReportCSVFileURL}">in Excel</a>
            <br />
            View the report <a href="{ReportXMLFileURL}">as XML</a>
        </xsl:if>     
           
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