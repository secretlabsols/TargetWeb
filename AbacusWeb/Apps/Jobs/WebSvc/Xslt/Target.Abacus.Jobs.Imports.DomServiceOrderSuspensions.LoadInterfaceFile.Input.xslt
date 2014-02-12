<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Dom Service Order Suspensions Import - Load Interface File Input.
		
		Formats the Imports.DomServiceOrderSuspensions.LoadInterfaceFile input information for display.
	-->
	<xsl:template match="File">
		The Dom Service Order Suspension interface file specified was: <xsl:value-of select="." /><br />
	</xsl:template>

</xsl:stylesheet>
