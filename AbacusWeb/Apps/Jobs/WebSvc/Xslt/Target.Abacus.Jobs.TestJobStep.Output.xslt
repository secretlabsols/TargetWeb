<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Test Job Step Output.
		
		Formats the TestJobStep output information for display.
	-->
	<xsl:template match="TestJobStep">
		The output file can be found at: <a href="{.}" target="_blank"><xsl:value-of select="." /></a><br />
	</xsl:template>
	
</xsl:stylesheet>
