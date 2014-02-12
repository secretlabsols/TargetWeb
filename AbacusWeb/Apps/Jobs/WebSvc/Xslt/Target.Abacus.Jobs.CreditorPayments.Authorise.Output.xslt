<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Authorise Creditor Payments
		
		Formats the output for the step
	-->
    <xsl:template match="Results">

        No. of payments retrieved: <xsl:value-of select="TotalPaymentToProcess" /><br />
        No. of payments authorised: <xsl:value-of select="TotalPaymentsProcessed" /><br />
        No. of payments failing authorisation: <xsl:value-of select="TotalExceptions" /><br />
        
        <xsl:if test="TotalPaymentsProcessed &gt; 0">
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
              
    </xsl:template>
</xsl:stylesheet>