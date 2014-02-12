<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
		<xsl:output method="html" indent="no" encoding="UTF-8" />
		<!--
		Abacus for Windows Job Service - Approve Care Home Payments
		
		Formats the output for the step
	-->
		<xsl:template match="Results">

			Number of items to approve: <xsl:value-of select="TotalItems" /><br />
			Number of items approved: <xsl:value-of select="TotalItemsSuccessful" /><br />
			Number of exceptions: <xsl:value-of select="TotalExceptions" /><br />

			<xsl:if test="TotalExceptions &gt; 0">
						<br />
						<b>Please see the exceptions report for more details:</b>
						<br />
						<br />
						View the report <a href="{ExceptionsReportCsvUrl}">in Excel</a>
						<br />
						View the report <a href="{ExceptionsReportXmlUrl}">as XML</a>
				</xsl:if>   
							
		</xsl:template>
</xsl:stylesheet>