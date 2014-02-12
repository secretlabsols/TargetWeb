<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ext="urn:ext-util">
    <xsl:output method="html" indent="no" encoding="ISO-8859-1" />
    <!--
		Abacus for Windows Job Service - Service Commitments Interface
		
		Formats the Commitments.GenerateData output information for display.
	-->
	<xsl:template match="Results">
		No. of Exceptions: <xsl:value-of select="ExceptionCount" /><br />
        <xsl:if test="ShowResInfo = 'Y'">
            Residential Commitment From: <xsl:value-of select="ResFrom" /><br />
            Residential Commitment To: <xsl:value-of select="ResTo" /><br />
        </xsl:if>
        <xsl:if test="ShowDPInfo = 'Y'">
            Direct Payment Commitment From: <xsl:value-of select="DPFrom" /><br />
            Direct Payment Commitment To: <xsl:value-of select="DPTo" /><br />
        </xsl:if>
        <xsl:if test="ShowResInfo = 'Y'">
            <br />
            Previous Residential Commitment negated: <xsl:value-of select="ext:FormatCurrency(TotalResNegated)" /><br />
            New Residential Commitment: <xsl:value-of select="ext:FormatCurrency(TotalRes)" /><br />
        </xsl:if>
        <xsl:if test="ShowDPInfo = 'Y'">
            <br />
            Previous Direct Payment Commitment negated: <xsl:value-of select="ext:FormatCurrency(TotalDPNegated)" /><br />
            New Direct Payment Commitment: <xsl:value-of select="ext:FormatCurrency(TotalDP)" /><br />
        </xsl:if>
        <b>
            <br />
            Total Commitment: <xsl:value-of select="ext:FormatCurrency(string(TotalOverall))" /><br />
        </b>
        <br />
		<xsl:if test="string-length(ExceptionsCSV_URL) > 0">
		    Please see the exceptions report for more details:<br />
			<a href="{ExceptionsCSV_URL}">View the report in Excel</a><br />
		</xsl:if>
        <br />
        <xsl:if test="string-length(ResultsSummaryCSV_URL) > 0 or string-length(ResultsDetailedCSV_URL) > 0">
            Please see the actions reports for more details:<br />
        </xsl:if>
        <xsl:if test="string-length(ResultsSummaryCSV_URL) > 0">
            <a href="{ResultsSummaryCSV_URL}">View the summary report in Excel</a><br />
        </xsl:if>
        <xsl:if test="string-length(ResultsDetailedCSV_URL) > 0">
            <a href="{ResultsDetailedCSV_URL}">View the detail report in Excel</a><br />            
        </xsl:if>
    </xsl:template>
	
</xsl:stylesheet>
