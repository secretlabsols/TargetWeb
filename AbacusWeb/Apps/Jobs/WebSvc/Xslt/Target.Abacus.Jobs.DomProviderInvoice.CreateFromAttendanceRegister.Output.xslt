<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Create Domiciliary Provider Invoice From Attendance Register
		
		Formats the DomProviderInvoice.CreateFromAttendanceRegister output information for display.
	-->
    <xsl:template match="Results">
        No. of registers processed: <xsl:value-of select="RegistersProcessed" /><br />
        No. of provider invoices successfully retracted: <xsl:value-of select="InvoiceRetractions" /><br />
        No. of provider invoices that could not be retracted: <xsl:value-of select="InvoiceRetractionFailures" /><br />
        No. of new provider invoices created: <xsl:value-of select="InvoicesCreated" /><br />
        No. of provider invoices that could not be created: <xsl:value-of select="InvoiceCreationFailures" /><br />
        <xsl:if test="string-length(ActionsCSVFileURL) &gt; 0">
            <br /><br />
            <b>The following file lists invoices sucessfully created invoices and any exceptions</b>
            <br />
            Please see the actions report for more details:
            <br />
            View the report <a href="{ActionsCSVFileURL}">in Excel</a>
            <br />
            View the report <a href="{ActionsXMLFileURL}">as XML</a>
        </xsl:if>
        <br /><br />
    </xsl:template>
</xsl:stylesheet>

