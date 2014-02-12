<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service Job Step Progress.
		
		Formats the job step progress information for display.
	-->
	<xsl:template match="progress">
		<xsl:for-each select="msg">
			<xsl:value-of disable-output-escaping="yes" select="concat(@date, ' - ', .)" /><br />
		</xsl:for-each>
	</xsl:template>
	
</xsl:stylesheet>
