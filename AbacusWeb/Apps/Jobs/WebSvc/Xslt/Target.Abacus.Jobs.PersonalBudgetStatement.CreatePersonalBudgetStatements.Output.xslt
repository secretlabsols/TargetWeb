<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Create Personal Budget Statements
		
		Formats the output for the step
	-->
    <xsl:template match="Results">

        Total No. of statements to create: <xsl:value-of select="StatementsToCreate" /><br />
        Total No. of statements successfully created: <xsl:value-of select="StatementsCreated" /><br />    
        Total exceptions (including reconsiderations): <xsl:value-of select="TotalExceptions" /><br />
        Total reconsideration exceptions: <xsl:value-of select="TotalReconsiderationExceptions" /><br />
        Total reconsideration warnings: <xsl:value-of select="TotalReconsiderationWarnings" />

        <xsl:if test="(TotalExceptions - TotalReconsiderationExceptions) &gt; 0">
            <br /><br />
            Please see the exceptions report for more details:
            <br />
            View the report <a href="{ExceptionReportCsvURL}">in Excel</a>
            <br />
            View the report <a href="{ExceptionReportXmlURL}">as XML</a>
        </xsl:if>

        <xsl:if test="TotalReconsiderationExceptions &gt; 0">
            <br /><br />
            Please see the reconsideration exceptions report for more details:
            <br />
            View the report <a href="{ReconsiderationExceptionCSVFileURL}">in Excel</a>
            <br />
            View the report <a href="{ReconsiderationExceptionXMLFileURL}">as XML</a>
        </xsl:if>

        <xsl:if test="TotalReconsiderationWarnings &gt; 0">
            <br /><br />
            Please see the reconsideration warnings report for more details:
            <br />
            View the report <a href="{ReconsiderationWarningCSVFileURL}">in Excel</a>
            <br />
            View the report <a href="{ReconsiderationWarningXMLFileURL}">as XML</a>
        </xsl:if>

    </xsl:template>
</xsl:stylesheet>