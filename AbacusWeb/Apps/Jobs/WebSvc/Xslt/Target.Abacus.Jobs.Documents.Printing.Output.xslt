<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Documents Printing
		
		Formats the output for the step
	-->
    <xsl:template match="Results">

        No. of documents printed: <xsl:value-of select="DocumentsPrinted" /><br />

        No. of documents could not be printed: <xsl:value-of select="DocumentsFailedToPrint" /><br />

        <xsl:if test="DocumentsFailedToPrint &gt; 0">
            <br />
            Please see the exceptions report for more details:
            <br />
            View the report <a href="{ExceptionCSVFileURL}">in Excel</a>
            <br />
            View the report <a href="{ExceptionXMLFileURL}">as XML</a>
        </xsl:if>   
              
    </xsl:template>
</xsl:stylesheet>