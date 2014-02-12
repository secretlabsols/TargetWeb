<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
		Abacus for Windows Job Service - Payments Import - Manage Payments - Output.
		
		Formats the Imports.Payments.ManagePayments output information for display.
	-->
	
	<xsl:template match="Results">
		<xsl:value-of select="PaymentCount" /> payment work record(s) were processed.
		<br /><br />
		<xsl:if test="number(TransactionsAddedCount) &gt; 0">
			<xsl:value-of select="TransactionsAddedCount" /> transaction(s) were added during processing.
			<br />
			Please see the additions report for more details:
			<br />
			View the report <a href="{AdditionsCsvFileUrl}">in Excel</a>
			<br />
			View the report <a href="{AdditionsXmlFileUrl}">as XML</a>
		</xsl:if>
		<br /><br />
		<xsl:if test="number(ExceptionsCount) &gt; 0">
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
	</xsl:template>
	
</xsl:stylesheet>
